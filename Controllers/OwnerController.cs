using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;
using System.Security.Claims;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.Owner)]
public class OwnerController(Csi402dbContext db, QuoteWorkService work, QuoteEstimateService estimates) : Controller
{
    private int OwnerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var month = ShopCalendar.MonthStartUtc;
        var model = new OwnerDashboardViewModel
        {
            MonthlyRevenue = await db.Invoices.Where(i => i.PaidAt >= month && i.Status == "ชำระเงินแล้ว").SumAsync(i => i.TotalAmount),
            OpenBudget = await db.Projects.Where(p => p.IsApproved && p.ProgressPercentage < 100).SumAsync(p => p.Budget),
            ActiveCustomProjects = await db.Projects.CountAsync(p => p.ProgressPercentage < 100 ||
                (p.UserId != null && p.Status != QuoteWorkflow.Ready && p.Status != QuoteWorkflow.LegacyDelivered)),
            TotalStaffCount = await db.Staff.CountAsync(s => s.Status == "active"),
            ProjectParts = (await db.ProjectParts.AsNoTracking().OrderBy(part => part.SlotName).ToListAsync())
                .ToLookup(part => part.ProjectCode),
            Projects = await db.Projects.AsNoTracking().OrderBy(p => p.Status != QuoteWorkflow.EstimateReview)
                .ThenBy(p => p.ProjectName).Select(p => new OwnerProjectRowViewModel
                {
                    Project = p,
                    LatestSubmittedEstimate = db.QuoteEstimates.AsNoTracking()
                        .Where(e => e.ProjectCode == p.ProjectCode && e.SubmittedAt != null)
                        .OrderByDescending(e => e.Revision).FirstOrDefault()
                }).ToListAsync(),
            Mechanics = await work.Mechanics().OrderBy(mechanic => mechanic.Name).ThenBy(mechanic => mechanic.Username).ToListAsync(),
            Departments = await OwnerDepartmentQuery.Summaries(db.Roles).ToListAsync()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EstimateDocument(string id, int revision)
    {
        var document = await estimates.Document(id, revision, submittedOnly: true);
        return document is null ? NotFound() : View("~/Views/Shared/QuoteEstimateDocument.cshtml", document);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnEstimate([Bind(Prefix = "ReturnForm")] QuoteEstimateReturnForm form)
    {
        if (!ModelState.IsValid) return BadRequest("กรุณาระบุเหตุผลที่ส่งกลับใบประเมิน");
        var result = await estimates.Return(form, OwnerId);
        if (result == QuoteWorkResult.NotFound) return NotFound();
        if (result != QuoteWorkResult.Updated) return Conflict("ใบประเมินฉบับนี้ไม่ได้อยู่ในคิวตรวจแล้ว");
        TempData["StatusMessage"] = "ส่งใบประเมินกลับให้ Accounting แก้ไขแล้ว";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(OwnerAssignmentViewModel assignmentForm)
    {
        if (!ModelState.IsValid) return BadRequest();
        var mechanic = await work.FindMechanic(assignmentForm.MechanicUserId, assignmentForm.Mechanic);
        if (mechanic is null) return BadRequest();
        var result = await work.Assign(assignmentForm, mechanic, OwnerId);
        if (result == QuoteWorkResult.NotFound) return NotFound();
        if (result == QuoteWorkResult.InvalidStage) return Conflict("มอบหมายไม่ได้ในสถานะปัจจุบัน กรุณาตรวจใบประเมินฉบับล่าสุด");
        TempData["StatusMessage"] = "มอบหมายช่างเรียบร้อยแล้ว";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deliver(QuoteDeliveryViewModel deliveryForm)
    {
        if (!ModelState.IsValid) return BadRequest();
        var result = await work.Deliver(deliveryForm.ProjectCode);
        if (result == QuoteWorkResult.NotFound) return NotFound();
        if (result == QuoteWorkResult.InvalidStage) return Conflict("ส่งงานได้หลังช่างทำงานครบ 100% และ Owner ตรวจแล้ว");
        TempData["StatusMessage"] = "อัปเดตสถานะพร้อมส่งงานให้ลูกค้าแล้ว";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProject([Bind(Prefix = "NewProjectForm")] OwnerCreateProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "กรุณากรอกข้อมูลโปรเจกต์ให้ครบถ้วน";
            return RedirectToAction(nameof(Index));
        }

        var newCode = $"PRJ-{Guid.NewGuid():N}";
        db.Projects.Add(new Project
        {
            ProjectCode = newCode,
            ProjectName = $"{model.ProjectName} ({model.CustomStyle})",
            CustomerName = model.CustomerName,
            Budget = model.Budget,
            Status = "อนุมัติเปิดโครงการใหม่",
            ProgressPercentage = 15,
            IsApproved = true
        });

        TempData["StatusMessage"] = $"สร้างและอนุมัติโปรเจกต์ {model.ProjectName} เรียบร้อยแล้ว (รหัส {newCode})";
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleApproval(string projectCode)
    {
        var proj = await db.Projects.FirstOrDefaultAsync(p => p.ProjectCode == projectCode);
        if (proj?.UserId != null) return Conflict("ใช้คำสั่งตามขั้นตอนของคำขอประเมินราคา");
        if (proj != null)
        {
            proj.IsApproved = !proj.IsApproved;
            proj.Status = proj.IsApproved ? "อนุมัติงบประมาณแล้ว" : "ระงับงบประมาณชั่วคราว";
            TempData["StatusMessage"] = $"อัปเดตสถานะการอนุมัติโปรเจกต์ {projectCode} สำเร็จ";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string projectCode, string newStatus, int progress)
    {
        var proj = await db.Projects.FirstOrDefaultAsync(p => p.ProjectCode == projectCode);
        if (proj?.UserId != null) return Conflict("ใช้คำสั่งตามขั้นตอนของคำขอประเมินราคา");
        if (proj != null)
        {
            proj.Status = newStatus;
            proj.ProgressPercentage = Math.Clamp(progress, 0, 100);
            if (await db.Jobs.SingleOrDefaultAsync(j => j.ProjectCode == projectCode) is { } job)
            {
                job.Status = proj.Status;
                job.ProgressPercent = proj.ProgressPercentage;
                job.CompletedAt = job.ProgressPercent == 100 ? job.CompletedAt ?? DateTime.UtcNow : null;
            }
            TempData["StatusMessage"] = $"ปรับปรุงความคืบหน้าโครงการ {projectCode} เป็น {progress}% เรียบร้อย";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
