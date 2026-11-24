using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.Mechanic)]
public class MechanicController(QuoteWorkService work) : Controller
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
            WaitingPartsCount = await jobs.CountAsync(j => j.Status.Contains("อะไหล่") && j.ProgressPercent < 100),
            JobOrders = await jobs.OrderBy(j => j.ProgressPercent).ToListAsync()
        };

        return View(model);
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
}
