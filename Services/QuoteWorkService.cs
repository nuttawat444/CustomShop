using System.Data;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public enum QuoteWorkResult { Updated, NotFound, Forbidden, InvalidStage }

public sealed class QuoteWorkService(Csi402dbContext db, QuoteEstimateService estimates)
{
    public IQueryable<MechanicOption> Mechanics() => db.Staff.AsNoTracking()
        .Where(staff => staff.Status == "active" && staff.Role.Name == AppRoles.Mechanic)
        .Select(staff => new MechanicOption { Id = staff.UserId, Name = staff.User.Name, Username = staff.User.Username });

    public async Task<MechanicOption?> FindMechanic(int? mechanicId, string? legacyName)
    {
        var matches = await Mechanics().Where(mechanic => mechanicId.HasValue
            ? mechanic.Id == mechanicId.Value : mechanic.Name == legacyName).Take(2).ToListAsync();
        return matches.Count == 1 ? matches[0] : null;
    }

    public IQueryable<MechanicJob> JobsFor(int mechanicId) =>
        from job in db.Jobs
        join assignment in db.JobAssignees on job.JobTicketNo equals assignment.JobTicketNo
        where assignment.MechanicUserId == mechanicId && Mechanics().Any(mechanic => mechanic.Id == mechanicId)
        select job;

    private async Task<MechanicJob?> LockJob(string ticket)
    {
        var jobs = await db.Jobs.FromSqlInterpolated($"SELECT * FROM shop_jobs WHERE JobTicketNo = {ticket} FOR UPDATE").ToListAsync();
        return jobs.SingleOrDefault();
    }

    public async Task<QuoteWorkResult> Assign(OwnerAssignmentViewModel form, MechanicOption mechanic, int ownerUserId)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var project = await db.LockQuoteProject(form.ProjectCode);
        if (project is null) return QuoteWorkResult.NotFound;
        if (!QuoteWorkflow.CanAssign(project)) return QuoteWorkResult.InvalidStage;
        if (QuoteWorkflow.CanReviewEstimate(project))
        {
            if (form.EstimateRevision is not int revision) return QuoteWorkResult.InvalidStage;
            var estimate = await estimates.PendingReview(project.ProjectCode, revision);
            if (estimate is null) return QuoteWorkResult.InvalidStage;

            estimate.ReviewedBy = ownerUserId;
            estimate.ReviewedAt = DateTime.UtcNow;
            project.Budget = estimate.TotalAmount;
        }
        else if (form.EstimateRevision.HasValue)
        {
            return QuoteWorkResult.InvalidStage;
        }
        else if (project.UserId is null)
        {
            project.Budget = form.Budget;
        }
        var job = await db.Jobs.SingleOrDefaultAsync(j => j.ProjectCode == project.ProjectCode);
        if (job is null)
        {
            job = new MechanicJob
            {
                JobTicketNo = $"JOB-{Guid.NewGuid():N}",
                ProjectCode = project.ProjectCode,
                BikeModel = project.ProjectName,
                ServiceType = project.ProjectName,
                ProgressPercent = project.ProgressPercentage
            };
            db.Jobs.Add(job);
        }
        job.AssignedMechanic = mechanic.Name;
        job.Status = job.ProgressPercent > 0 ? QuoteWorkflow.InProgress : QuoteWorkflow.Assigned;
        project.IsApproved = true;
        project.Status = job.Status;
        await db.SaveChangesAsync();

        var assignment = await db.JobAssignees.FindAsync(job.JobTicketNo);
        if (assignment is null)
            db.JobAssignees.Add(new JobAssignee { JobTicketNo = job.JobTicketNo, MechanicUserId = mechanic.Id });
        else
            assignment.MechanicUserId = mechanic.Id;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return QuoteWorkResult.Updated;
    }

    public async Task<QuoteWorkResult> Advance(int mechanicId, JobProgressViewModel form)
    {
        var ticket = await JobsFor(mechanicId).AsNoTracking().Where(job => job.JobTicketNo == form.TicketNo)
            .Select(job => new { job.ProjectCode }).SingleOrDefaultAsync();
        if (ticket is null) return QuoteWorkResult.Forbidden;

        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        // Assignment, progress and delivery acquire the project row first for linked jobs.
        var project = ticket.ProjectCode is null ? null : await db.LockQuoteProject(ticket.ProjectCode);
        var job = await LockJob(form.TicketNo);
        if (job is null) return QuoteWorkResult.NotFound;
        if (!await JobsFor(mechanicId).AnyAsync(assigned => assigned.JobTicketNo == job.JobTicketNo))
            return QuoteWorkResult.Forbidden;
        if (!QuoteWorkflow.Advance(job, project, form.Complete == true, DateTime.UtcNow))
            return QuoteWorkResult.InvalidStage;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return QuoteWorkResult.Updated;
    }

    public async Task<QuoteWorkResult> Deliver(string projectCode)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var project = await db.LockQuoteProject(projectCode);
        if (project is null) return QuoteWorkResult.NotFound;
        if (!QuoteWorkflow.CanDeliver(project)) return QuoteWorkResult.InvalidStage;
        var job = await db.Jobs.SingleOrDefaultAsync(j => j.ProjectCode == project.ProjectCode);
        if (!QuoteWorkflow.Deliver(project, job)) return QuoteWorkResult.InvalidStage;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return QuoteWorkResult.Updated;
    }
}
