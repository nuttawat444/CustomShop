using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.WarehouseWorker)]
public class WarehouseWorkerController(Csi402dbContext db, WarehouseCatalogService warehouseCatalog) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var today = ShopCalendar.TodayUtc;
        var model = new WarehouseWorkerDashboardViewModel
        {
            TotalStockItems = await db.Stock.SumAsync(i => i.QuantityOnHand),
            LowStockAlerts = await db.Stock.CountAsync(i => i.QuantityOnHand <= i.MinRequired),
            PendingShipments = await db.WarehouseTasks.CountAsync(t => !t.IsCompleted && t.SaleOrderId != null),
            CompletedTasksToday = await db.WarehouseTasks.CountAsync(t => t.CompletedAt >= today),
            InventoryItems = await db.Stock.AsNoTracking().OrderBy(i => i.Id).ToListAsync(),
            ProductVisibility = await db.Catalog.AsNoTracking().Where(item => item.Kind == "Product" || item.Kind == "Package")
                .ToDictionaryAsync(item => item.Id, item => item.Active),
            DailyTasks = await db.WarehouseTasks.AsNoTracking().OrderBy(t => t.IsCompleted).ThenBy(t => t.Id).ToListAsync()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> PackingList(int id)
    {
        var order = await db.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        if (order == null) return NotFound();
        var lines = await db.OrderLines.AsNoTracking().Where(x => x.SaleOrderId == id).OrderBy(x => x.Id).ToListAsync();
        return View(new OrderReceipt(order, lines));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(StockAdjustmentViewModel stockForm)
    {
        if (!ModelState.IsValid) return BadRequest();
        var amount = stockForm.Amount;
        var change = stockForm.ActionType == "Restock" ? amount : -amount;
        var count = await db.Stock.Where(x => x.Id == stockForm.Id && (change > 0 ? x.QuantityOnHand <= int.MaxValue - amount : x.QuantityOnHand >= amount))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.QuantityOnHand, x => x.QuantityOnHand + change));
        TempData[count == 1 ? "StatusMessage" : "ErrorMessage"] = count == 1 ? "ปรับสต็อกแล้ว" : "ปรับยอดไม่ได้ กรุณาตรวจสอบจำนวนคงเหลือ";
        return RedirectToAction(nameof(Index), null, null, $"stock-{stockForm.Id}");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem([Bind(Prefix = "NewItemForm")] WarehouseWorkerAddItemViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return InvalidListing();
        var result = await warehouseCatalog.Add(model, cancellationToken);
        return ListingResponse(result, model.Listing.PublishNow);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ListExisting([Bind(Prefix = "ExistingListingForm")] WarehouseExistingListingViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return InvalidListing();
        var result = await warehouseCatalog.ListExisting(model, cancellationToken);
        return ListingResponse(result, model.Listing.PublishNow);
    }

    private IActionResult InvalidListing()
    {
        TempData["ErrorMessage"] = ModelState.Values.SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage).FirstOrDefault(message => !string.IsNullOrWhiteSpace(message))
            ?? "กรุณาตรวจสอบข้อมูลสินค้าและราคาขาย";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult ListingResponse(WarehouseListingResult result, bool published)
    {
        switch (result)
        {
            case WarehouseListingResult.Added:
                TempData["StatusMessage"] = published ? "เปิดขายสินค้าบนหน้า Product แล้ว" : "บันทึกสินค้าแล้ว รอ Admin ตรวจราคาและเปิดขาย";
                break;
            case WarehouseListingResult.DuplicateSku:
                TempData["ErrorMessage"] = "รหัสสินค้านี้มีอยู่แล้ว";
                break;
            case WarehouseListingResult.StockNotFound:
                return NotFound();
            case WarehouseListingResult.AlreadyLinked:
                TempData["ErrorMessage"] = "SKU นี้เชื่อมกับหน้าร้านแล้ว";
                break;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleTask(int taskId)
    {
        var task = await db.WarehouseTasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;
            TempData["StatusMessage"] = $"อัปเดตสถานะงาน '{task.TaskTitle}' เรียบร้อยแล้ว";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
