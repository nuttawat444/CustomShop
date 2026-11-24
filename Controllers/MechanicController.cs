using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.Mechanic)]
public class MechanicController(Csi402dbContext db, QuoteWorkService work) : Controller
{
    private int MechanicId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var today = ShopCalendar.TodayUtc;
        var jobs = work.JobsFor(MechanicId).AsNoTracking();
        var model = new MechanicDashboardViewModel
        {
            ActiveJobsCount = await jobs.CountAsync(j => j.ProgressPercent < 100),
            CompletedTodayCount = await jobs.CountAsync(j => j.CompletedAt >= today),
            UrgentJobsCount = await jobs.CountAsync(j => j.Priority == "Urgent" && j.ProgressPercent < 100),
            WaitingPartsCount = await jobs.CountAsync(j => j.Status.Contains("อะไหล่") && j.ProgressPercent < 100),
            Mechanics = await work.Mechanics().OrderBy(mechanic => mechanic.Name).ThenBy(mechanic => mechanic.Username).ToListAsync(),
            JobOrders = await jobs.OrderBy(j => j.ProgressPercent).ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJob([Bind(Prefix = "NewJobForm")] MechanicCreateJobViewModel model)
    {
        var mechanic = await work.FindMechanic(model.MechanicUserId, model.AssignedMechanic);
        if (!ModelState.IsValid || mechanic is null)
        {
            TempData["ErrorMessage"] = "กรุณากรอกข้อมูลใบสั่งงานช่างให้ครบถ้วน";
            return RedirectToAction(nameof(Index));
        }

        var newTicket = $"JOB-{Guid.NewGuid():N}";
        await using var tx = await db.Database.BeginTransactionAsync();
        db.Jobs.Add(new MechanicJob
        {
            JobTicketNo = newTicket,
            BikeModel = model.BikeModel.Trim(),
            CustomStyle = model.CustomStyle,
            ServiceType = model.ServiceType.Trim(),
            AssignedMechanic = mechanic.Name,
            Priority = model.Priority,
            Status = "เปิดใบงานใหม่",
            ProgressPercent = 10
        });

        TempData["StatusMessage"] = $"เปิดใบสั่งงานคัสตอม {newTicket} สำหรับ {model.BikeModel} เรียบร้อยแล้ว";
        await db.SaveChangesAsync();
        db.JobAssignees.Add(new JobAssignee { JobTicketNo = newTicket, MechanicUserId = mechanic.Id });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProgress(JobProgressViewModel progressForm)
    {
        if (!ModelState.IsValid) return BadRequest();
        var result = await work.Advance(MechanicId, progressForm);
        if (result == QuoteWorkResult.Forbidden) return Forbid();
        if (result == QuoteWorkResult.NotFound) return NotFound();
        if (result == QuoteWorkResult.InvalidStage) return Conflict("ใบงานนี้อัปเดตความคืบหน้าไม่ได้ในสถานะปัจจุบัน");
        TempData["StatusMessage"] = "บันทึกความคืบหน้าแล้ว";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePriority(string ticketNo, string priority)
    {
        if (priority is not ("Normal" or "High" or "Urgent")) return BadRequest();
        var job = await work.JobsFor(MechanicId).FirstOrDefaultAsync(j => j.JobTicketNo == ticketNo);
        if (job is null) return Forbid();
        if (job != null)
        {
            job.Priority = priority;
            TempData["StatusMessage"] = $"ปรับระดับความสำคัญใบงาน {ticketNo} เป็น '{priority}' สำเร็จ";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
