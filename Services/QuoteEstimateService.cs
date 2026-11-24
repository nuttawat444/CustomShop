using System.Data;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public sealed class QuoteEstimateService(Csi402dbContext db)
{
    public Task<List<QuoteEstimateQueueRowViewModel>> Queue() => db.Projects.AsNoTracking()
        .Where(project => project.UserId != null && !project.IsApproved && project.ProgressPercentage == 0 &&
            (project.Status == QuoteWorkflow.Pending || project.Status == QuoteWorkflow.Estimating ||
             project.Status == QuoteWorkflow.EstimateReview || project.Status == QuoteWorkflow.EstimateReturned))
        .OrderBy(project => project.Status == QuoteWorkflow.EstimateReview)
        .ThenBy(project => project.ProjectName).ThenBy(project => project.ProjectCode)
        .Select(project => new QuoteEstimateQueueRowViewModel
        {
            Project = project,
            LatestRevision = db.QuoteEstimates.Where(e => e.ProjectCode == project.ProjectCode).Max(e => (int?)e.Revision)
        }).ToListAsync();

    public async Task<QuoteEstimateEditorViewModel?> Editor(string projectCode)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var project = await db.Projects.AsNoTracking().SingleOrDefaultAsync(p => p.ProjectCode == projectCode);
        if (project is null || !QuoteWorkflow.CanEstimate(project)) return null;

        var history = await db.QuoteEstimates.AsNoTracking().Where(e => e.ProjectCode == projectCode)
            .OrderByDescending(e => e.Revision).ToListAsync();
        var latest = history.FirstOrDefault();
        var prices = latest is null ? new Dictionary<string, decimal>() :
            await db.QuoteEstimateLines.AsNoTracking().Where(line =>
                line.ProjectCode == projectCode && line.Revision == latest.Revision)
                .ToDictionaryAsync(line => line.SlotCode, line => line.UnitPrice);
        var parts = await Parts(projectCode);
        var form = new QuoteEstimateForm
        {
            ProjectCode = project.ProjectCode,
            Revision = latest?.Revision ?? 0,
            EditVersion = latest?.EditVersion ?? 0,
            LaborAmount = latest?.LaborAmount ?? 0,
            OtherAmount = latest?.OtherAmount ?? (parts.Count == 0 ? project.Budget : 0),
            DiscountAmount = latest?.DiscountAmount ?? 0,
            Notes = latest?.Notes ?? string.Empty,
            Lines = parts.Select(part => new QuoteEstimatePriceForm
            {
                SlotCode = part.SlotCode,
                UnitPrice = prices.GetValueOrDefault(part.SlotCode, part.UnitPrice)
            }).ToList()
        };
        await tx.CommitAsync();
        return new QuoteEstimateEditorViewModel { Project = project, Parts = parts, History = history, Form = form };
    }

    public async Task<QuoteEstimateDocumentViewModel?> Document(string projectCode, int revision, bool submittedOnly)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var project = await db.Projects.AsNoTracking().SingleOrDefaultAsync(p => p.ProjectCode == projectCode && p.UserId != null);
        if (project is null) return null;
        var estimate = await db.QuoteEstimates.AsNoTracking().SingleOrDefaultAsync(e =>
            e.ProjectCode == projectCode && e.Revision == revision && (!submittedOnly || e.SubmittedAt != null));
        if (estimate is null) return null;
        var lines = await db.QuoteEstimateLines.AsNoTracking().Where(line =>
            line.ProjectCode == projectCode && line.Revision == revision).OrderBy(line => line.SlotCode).ToListAsync();
        await tx.CommitAsync();
        return new QuoteEstimateDocumentViewModel { Project = project, Estimate = estimate, Lines = lines };
    }

    public async Task<int> Save(QuoteEstimateForm form, int accountingUserId, bool submit)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var project = await db.LockQuoteProject(form.ProjectCode);
        if (project is null || !QuoteWorkflow.CanEstimate(project))
            throw new ShopException("คำขอนี้ไม่อยู่ในขั้นจัดทำใบประเมินแล้ว");

        var latest = await db.QuoteEstimates.Where(e => e.ProjectCode == project.ProjectCode)
            .OrderByDescending(e => e.Revision).FirstOrDefaultAsync();
        if (!QuoteEstimateRules.MatchesVersion(latest, form.Revision, form.EditVersion))
            throw new ShopException("ใบประเมินถูกแก้ไขแล้ว กรุณาเปิดใบประเมินล่าสุดและตรวจข้อมูลก่อนบันทึก");

        var parts = await Parts(project.ProjectCode);
        var prices = QuoteEstimateRules.PricesForParts(parts, form.Lines);
        var total = QuoteEstimateRules.Total(parts.Select(part => (part.Quantity, prices[part.SlotCode])).ToArray(),
            form.LaborAmount, form.OtherAmount, form.DiscountAmount);

        var estimate = latest?.SubmittedAt is null ? latest : null;
        var newRevision = estimate is null;
        if (estimate is null)
        {
            estimate = new QuoteEstimate { ProjectCode = project.ProjectCode, Revision = (latest?.Revision ?? 0) + 1 };
            db.QuoteEstimates.Add(estimate);
        }
        estimate.EditVersion++;
        estimate.LaborAmount = form.LaborAmount;
        estimate.OtherAmount = form.OtherAmount;
        estimate.DiscountAmount = form.DiscountAmount;
        estimate.TotalAmount = total;
        estimate.Notes = form.Notes?.Trim() ?? string.Empty;
        estimate.UpdatedAt = DateTime.UtcNow;
        estimate.UpdatedBy = accountingUserId;
        estimate.SubmittedAt = submit ? estimate.UpdatedAt : null;
        project.Status = submit ? QuoteWorkflow.EstimateReview : QuoteWorkflow.Estimating;

        if (newRevision)
        {
            await db.SaveChangesAsync();
            foreach (var part in parts)
                db.QuoteEstimateLines.Add(new QuoteEstimateLine
                {
                    ProjectCode = project.ProjectCode, Revision = estimate.Revision, SlotCode = part.SlotCode,
                    PartName = part.PartName, Sku = part.Sku, Variant = part.Variant, Quantity = part.Quantity,
                    UnitPrice = prices[part.SlotCode]
                });
        }
        else
        {
            var lines = await db.QuoteEstimateLines.Where(line =>
                line.ProjectCode == project.ProjectCode && line.Revision == estimate.Revision).ToListAsync();
            foreach (var line in lines)
                line.UnitPrice = prices[line.SlotCode];
        }
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return estimate.Revision;
    }

    public async Task<QuoteWorkResult> Return(QuoteEstimateReturnForm form, int ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(form.Reason) || form.Reason.Length > 1000)
            return QuoteWorkResult.InvalidStage;

        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var project = await db.LockQuoteProject(form.ProjectCode);
        if (project is null) return QuoteWorkResult.NotFound;
        if (!QuoteWorkflow.CanReviewEstimate(project)) return QuoteWorkResult.InvalidStage;

        var estimate = await PendingReview(project.ProjectCode, form.Revision);
        if (estimate is null) return QuoteWorkResult.InvalidStage;

        estimate.ReviewedBy = ownerUserId;
        estimate.ReviewedAt = DateTime.UtcNow;
        estimate.ReturnReason = form.Reason.Trim();
        project.Status = QuoteWorkflow.EstimateReturned;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return QuoteWorkResult.Updated;
    }

    internal async Task<QuoteEstimate?> PendingReview(string projectCode, int revision)
    {
        var latest = await db.QuoteEstimates.Where(e => e.ProjectCode == projectCode)
            .OrderByDescending(e => e.Revision).FirstOrDefaultAsync();
        return QuoteEstimateRules.IsAwaitingReview(latest, revision) ? latest : null;
    }

    private Task<List<ProjectPart>> Parts(string code) => db.ProjectParts.AsNoTracking()
        .Where(part => part.ProjectCode == code).OrderBy(part => part.SlotCode).ToListAsync();
}
