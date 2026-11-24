using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;
using System.Security.Claims;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.Account)]
public class AccountingController(Csi402dbContext db, QuoteEstimateService estimates) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Estimates() => View(await estimates.Queue());

    [HttpGet]
    public async Task<IActionResult> Estimate(string id)
    {
        var page = await estimates.Editor(id);
        return page is null ? NotFound() : View(page);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEstimate([Bind(Prefix = "Form")] QuoteEstimateForm form, bool submit)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var revision = await estimates.Save(form, userId, submit);
                TempData["StatusMessage"] = submit ? "ส่งใบประเมินให้ Owner แล้ว" : "บันทึกร่างใบประเมินแล้ว";
                return submit
                    ? RedirectToAction(nameof(EstimateDocument), new { id = form.ProjectCode, revision })
                    : RedirectToAction(nameof(Estimate), new { id = form.ProjectCode });
            }
            catch (ShopException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }

        var page = await estimates.Editor(form.ProjectCode);
        if (page is null || !QuoteEstimateRules.MatchesVersion(page.History.FirstOrDefault(), form.Revision, form.EditVersion))
            return Conflict("ใบประเมินหรือสถานะคำขอเปลี่ยนแล้ว กรุณาเปิดจากหน้าคิวประเมินราคาอีกครั้ง");
        return View(nameof(Estimate), page);
    }

    [HttpGet]
    public async Task<IActionResult> EstimateDocument(string id, int revision)
    {
        var document = await estimates.Document(id, revision, submittedOnly: false);
        return document is null ? NotFound() : View("~/Views/Shared/QuoteEstimateDocument.cshtml", document);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var today = ShopCalendar.TodayUtc;
        var month = ShopCalendar.MonthStartUtc;
        var paid = db.Invoices.Where(i => i.Status == "ชำระเงินแล้ว");
        var model = new AccountingDashboardViewModel
        {
            TodayRevenue = await paid.Where(i => i.PaidAt >= today).SumAsync(i => i.TotalAmount),
            MonthToDateRevenue = await paid.Where(i => i.PaidAt >= month).SumAsync(i => i.TotalAmount),
            OutstandingInvoices = await db.Invoices.Where(i => i.Status == "รอชำระเงิน").SumAsync(i => i.TotalAmount),
            TotalCouponsRedeemed = await db.Orders.CountAsync(o => o.CouponCode != ""),
            Invoices = await db.Invoices.AsNoTracking().OrderByDescending(i => i.CreatedAt).ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateInvoice([Bind(Prefix = "NewInvoiceForm")] AccountingCreateInvoiceViewModel model)
    {
        if (!ModelState.IsValid || model.DiscountAmount < 0 || model.DiscountAmount > model.TotalAmount)
        {
            TempData["ErrorMessage"] = "กรุณากรอกข้อมูลใบเสร็จรับเงินให้ถูกต้องครบถ้วน";
            return RedirectToAction(nameof(Index));
        }

        var newInvNo = $"INV-{Guid.NewGuid():N}";
        db.Invoices.Add(new Invoice
        {
            InvoiceNo = newInvNo,
            CustomerName = model.CustomerName.Trim(),
            ItemDescription = model.ItemDescription.Trim(),
            TotalAmount = Math.Max(0, model.TotalAmount - model.DiscountAmount),
            DiscountAmount = model.DiscountAmount,
            PaymentMethod = model.PaymentMethod,
            Status = model.PaymentMethod == "รอวางบิล" ? "รอชำระเงิน" : "ชำระเงินแล้ว",
            CreatedAt = DateTime.UtcNow,
            PaidAt = model.PaymentMethod == "รอวางบิล" ? null : DateTime.UtcNow
        });

        TempData["StatusMessage"] = $"ออกใบเสร็จรับเงิน {newInvNo} สำหรับคุณ {model.CustomerName} เรียบร้อยแล้ว";
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(string invoiceNo)
    {
        var inv = await db.Invoices.FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo);
        if (inv?.Status == "รอชำระเงิน")
        {
            inv.PaidAt = DateTime.UtcNow;
            inv.Status = "ชำระเงินแล้ว";
            TempData["StatusMessage"] = $"บันทึกการชำระเงินใบแจ้งหนี้ {invoiceNo} สำเร็จ";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelInvoice(string invoiceNo)
    {
        var inv = await db.Invoices.FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo);
        if (inv != null && inv.SaleOrderId == null)
        {
            inv.Status = "ยกเลิกบิล";
            TempData["StatusMessage"] = $"ยกเลิกใบเสร็จ/ใบแจ้งหนี้ {invoiceNo} เรียบร้อยแล้ว";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
