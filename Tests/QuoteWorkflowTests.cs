using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;
using Xunit;

namespace CustomShop.Tests;

public class QuoteWorkflowTests
{
    [Theory]
    [InlineData(QuoteWorkflow.Pending, 0, QuoteWorkflow.Pending, 1)]
    [InlineData(QuoteWorkflow.Estimating, 0, QuoteWorkflow.Estimating, 2)]
    [InlineData(QuoteWorkflow.EstimateReturned, 0, QuoteWorkflow.Estimating, 2)]
    [InlineData(QuoteWorkflow.EstimateReview, 0, QuoteWorkflow.EstimateReview, 2)]
    [InlineData(QuoteWorkflow.Assigned, 0, QuoteWorkflow.Assigned, 3)]
    [InlineData("ประกอบเสร็จและทดสอบการขับขี่", 25, QuoteWorkflow.InProgress, 4)]
    [InlineData(QuoteWorkflow.InProgress, 99, QuoteWorkflow.InProgress, 4)]
    [InlineData("งานเสร็จสมบูรณ์พร้อมส่งมอบ", 100, QuoteWorkflow.AwaitingInspection, 4)]
    [InlineData(QuoteWorkflow.LegacyDelivered, 100, QuoteWorkflow.Ready, 5)]
    public void PortalUsesSameStatusForNewAndExistingQuotes(string stored, int progress, string status, int step)
    {
        var project = new Project { Status = stored, ProgressPercentage = progress };

        Assert.Equal(status, QuoteWorkflow.Status(project));
        Assert.Equal(step, QuoteWorkflow.Step(project));
    }

    [Theory]
    [InlineData(0, false, 25, QuoteWorkflow.InProgress)]
    [InlineData(74, false, 99, QuoteWorkflow.InProgress)]
    [InlineData(75, false, 100, QuoteWorkflow.AwaitingInspection)]
    [InlineData(0, true, 100, QuoteWorkflow.AwaitingInspection)]
    public void AdvanceSetsTheSameResultOnProjectAndJob(int start, bool complete, int expected, string status)
    {
        var project = new Project { UserId = 1, Status = QuoteWorkflow.Assigned, ProgressPercentage = start };
        var job = new MechanicJob { ProgressPercent = start };
        var now = DateTime.UtcNow;

        var changed = QuoteWorkflow.Advance(job, project, complete, now);

        Assert.True(changed);
        Assert.Equal(expected, job.ProgressPercent);
        Assert.Equal(expected, project.ProgressPercentage);
        Assert.Equal(status, project.Status);
        Assert.Equal(status, job.Status);
        Assert.Equal(expected == 100 ? now : (DateTime?)null, job.CompletedAt);
    }

    [Theory]
    [InlineData(QuoteWorkflow.Pending, 0)]
    [InlineData(QuoteWorkflow.Estimating, 0)]
    [InlineData(QuoteWorkflow.EstimateReview, 0)]
    [InlineData(QuoteWorkflow.EstimateReturned, 0)]
    [InlineData(QuoteWorkflow.AwaitingInspection, 100)]
    [InlineData(QuoteWorkflow.Ready, 100)]
    public void AdvanceRejectsStatesOutsideMechanicWork(string state, int progress)
    {
        var project = new Project { UserId = 1, Status = state, ProgressPercentage = progress };
        var job = new MechanicJob { ProgressPercent = progress };

        var changed = QuoteWorkflow.Advance(job, project, true, DateTime.UtcNow);

        Assert.False(changed);
        Assert.Equal(state, project.Status);
        Assert.Equal(progress, job.ProgressPercent);
    }

    [Theory]
    [InlineData(QuoteWorkflow.Pending, false, false, true, false, false)]
    [InlineData(QuoteWorkflow.Estimating, false, false, true, false, false)]
    [InlineData(QuoteWorkflow.EstimateReturned, false, false, true, false, false)]
    [InlineData(QuoteWorkflow.EstimateReview, false, true, false, true, false)]
    [InlineData(QuoteWorkflow.Assigned, true, true, false, false, true)]
    [InlineData(QuoteWorkflow.Ready, true, false, false, false, true)]
    public void CustomerProjectHasSeparateAccountingAndOwnerPermissions(
        string status, bool approved, bool assign, bool edit, bool review, bool showPrice)
    {
        var project = new Project { UserId = 3, Status = status, IsApproved = approved };

        Assert.Equal(assign, QuoteWorkflow.CanAssign(project));
        Assert.Equal(edit, QuoteWorkflow.CanEstimate(project));
        Assert.Equal(review, QuoteWorkflow.CanReviewEstimate(project));
        Assert.Equal(showPrice, QuoteWorkflow.CanShowConfirmedPrice(project));
    }

    [Fact]
    public void InternalProjectStillUsesOwnerAssignmentWithoutEstimate()
    {
        var project = new Project { Status = "อนุมัติเปิดโครงการใหม่", ProgressPercentage = 15, IsApproved = true };

        Assert.True(QuoteWorkflow.CanAssign(project));
        Assert.False(QuoteWorkflow.CanEstimate(project));
        Assert.False(QuoteWorkflow.CanReviewEstimate(project));
    }

    [Fact]
    public void DeliveryRequiresBothCustomerProjectAndCompletedJobAndCannotRunTwice()
    {
        var project = new Project { UserId = 1, Status = QuoteWorkflow.AwaitingInspection, ProgressPercentage = 100 };
        var job = new MechanicJob { ProgressPercent = 99 };

        Assert.False(QuoteWorkflow.Deliver(project, job));
        job.ProgressPercent = 100;
        var delivered = QuoteWorkflow.Deliver(project, job);

        Assert.True(delivered);
        Assert.Equal(QuoteWorkflow.Ready, project.Status);
        Assert.Equal(QuoteWorkflow.Ready, job.Status);
        Assert.False(QuoteWorkflow.Deliver(project, job));
        Assert.False(QuoteWorkflow.CanAssign(project));
    }
}
