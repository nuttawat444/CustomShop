using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.Services;

public static class QuoteWorkflow
{
    public const string Pending = "รอประเมินราคา";
    public const string Estimating = "กำลังประเมินราคา";
    public const string EstimateReview = "รอ Owner ตรวจราคา";
    public const string EstimateReturned = "ส่งกลับแก้ใบประเมิน";
    public const string Assigned = "รับงานแล้ว";
    public const string InProgress = "กำลังดำเนินงาน";
    public const string AwaitingInspection = "ประกอบเสร็จสิ้นทดลองแล้ว";
    public const string Ready = "พร้อมส่งงาน";
    public const string LegacyDelivered = "งานประกอบเสร็จสมบูรณ์พร้อมส่งมอบ";

    public static bool CanEstimate(Project project) => project.UserId.HasValue && !project.IsApproved &&
        project.ProgressPercentage == 0 && project.Status is Pending or Estimating or EstimateReturned;

    public static string Status(Project project) =>
        project.Status is Ready or LegacyDelivered ? Ready :
        project.ProgressPercentage >= 100 ? AwaitingInspection :
        project.ProgressPercentage > 0 ? InProgress :
        project.Status switch
        {
            Assigned => Assigned,
            EstimateReview => EstimateReview,
            Estimating or EstimateReturned => Estimating,
            _ => Pending
        };

    public static int Step(Project project) => Status(project) switch
    {
        Pending => 1,
        Estimating or EstimateReview => 2,
        Assigned => 3,
        Ready => 5,
        _ => 4
    };

    public static bool CanAssign(Project project) => project.ProgressPercentage < 100 &&
        (project.UserId == null || CanReviewEstimate(project) ||
         (project.IsApproved && Status(project) is Assigned or InProgress));

    public static bool CanReviewEstimate(Project project) => project.UserId.HasValue && !project.IsApproved &&
        project.ProgressPercentage == 0 && project.Status == EstimateReview;

    public static bool CanShowConfirmedPrice(Project project) => project.IsApproved && Step(project) >= 3;

    public static bool CanDeliver(Project project) => project.UserId.HasValue && Status(project) == AwaitingInspection;

    public static bool Advance(MechanicJob job, Project? project, bool complete, DateTime now)
    {
        if (job.ProgressPercent >= 100 ||
            (project is not null && project.UserId.HasValue && Status(project) is not (Assigned or InProgress)))
            return false;

        job.ProgressPercent = complete ? 100 : Math.Min(job.ProgressPercent + 25, 100);
        job.Status = job.ProgressPercent == 100 ? AwaitingInspection : InProgress;
        if (job.ProgressPercent == 100)
            job.CompletedAt = now;
        if (project is not null)
        {
            project.ProgressPercentage = job.ProgressPercent;
            project.Status = job.Status;
        }
        return true;
    }

    public static bool Deliver(Project project, MechanicJob? job)
    {
        if (!CanDeliver(project) || job?.ProgressPercent != 100)
            return false;
        project.Status = Ready;
        job.Status = Ready;
        return true;
    }
}
