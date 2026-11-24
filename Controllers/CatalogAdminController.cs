using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;
using Nuttawat_7324_L002.Services;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[AutoValidateAntiforgeryToken]
public class CatalogAdminController(Csi402dbContext db, IWebHostEnvironment env) : Controller
{
    private string[] Images() => Directory.GetFiles(Path.Combine(env.WebRootPath, "images", "shop"), "*.jpg")
        .Select(Path.GetFileName).OfType<string>().Order(StringComparer.Ordinal).ToArray();

    [HttpGet]
    public async Task<IActionResult> Index(CatalogAdminSection section = CatalogAdminSection.Product,
        int? edit = null, bool create = false, string? couponCode = null)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(section)) return BadRequest();
        var page = await BuildPage(section);
        if (section == CatalogAdminSection.Coupons)
        {
            if (create) page.CouponForm = new CouponAdminForm { Active = true };
            else if (couponCode is not null)
            {
                var coupon = page.Coupons.SingleOrDefault(item => item.Code == couponCode);
                if (coupon is null) return NotFound();
                page.CouponForm = new CouponAdminForm
                {
                    Code = coupon.Code, Percent = coupon.Percent, Category = coupon.Category, Active = coupon.Active
                };
            }
        }
        else if (create)
        {
            page.ItemForm = new CatalogItemAdminForm { Kind = CatalogAdminSections.Kind(section)!, Active = true, PinOnHome = false };
        }
        else if (edit.HasValue)
        {
            var item = page.Items.SingleOrDefault(item => item.Id == edit);
            if (item is null) return NotFound();
            page.ItemForm = CatalogItemAdminForm.FromItem(item);
            page.ItemForm.PinOnHome = page.PinnedItemIds.Contains(item.Id);
        }
        return View(page);
    }

    private async Task<CatalogAdminViewModel> BuildPage(CatalogAdminSection section)
    {
        if (section == CatalogAdminSection.Coupons)
        {
            return new CatalogAdminViewModel
            {
                Section = section,
                Coupons = await db.Coupons.AsNoTracking().OrderBy(coupon => coupon.Code).ToListAsync(),
                ProductCategories = await db.Catalog.Where(item => item.Kind == "Product")
                    .Select(item => item.Category).Distinct().OrderBy(category => category).ToListAsync()
            };
        }
        var kind = CatalogAdminSections.Kind(section);
        return new CatalogAdminViewModel
        {
            Section = section,
            Images = Images(),
            PinnedItemIds = (await (from pin in db.HomePins
                                   join item in db.Catalog on pin.CatalogItemId equals item.Id
                                   where pin.IsPinned && item.Kind == kind
                                   select pin.CatalogItemId).ToListAsync()).ToHashSet(),
            Items = await db.Catalog.AsNoTracking().Where(item => item.Kind == kind)
                .OrderByDescending(item => item.Id).ToListAsync()
        };
    }

    [HttpPost]
    public async Task<IActionResult> Save(CatalogItemAdminForm itemForm)
    {
        var section = CatalogAdminSections.ForKind(itemForm.Kind);
        if (!Images().Contains(itemForm.Image, StringComparer.Ordinal))
            ModelState.AddModelError("ItemForm.Image", "กรุณาเลือกรูปจากคลังรูปของร้าน");
        if (!ModelState.IsValid) return await ItemError(section, itemForm);

        var category = itemForm.Category.Trim();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var item = itemForm.Id == 0
            ? new CatalogItem { Kind = itemForm.Kind, Slug = Guid.NewGuid().ToString("N") }
            : (await db.Catalog.FromSqlInterpolated($"SELECT * FROM shop_catalog WHERE Id = {itemForm.Id} FOR UPDATE")
                .ToListAsync()).SingleOrDefault();
        if (item is null) return NotFound();
        if (item.Kind != itemForm.Kind || (itemForm.Id != 0 && item.Kind == "Product" &&
            (category == "helmet") != (item.Category == "helmet")))
            return BadRequest("การเปลี่ยนประเภทหรือหมวดที่มีผลต่อไซซ์ต้องสร้างรายการใหม่");

        item.Name = itemForm.Name.Trim();
        item.Category = category;
        item.Image = itemForm.Image;
        item.Description = itemForm.Description?.Trim() ?? "";
        item.Features = itemForm.Features?.Trim() ?? "";
        item.Price = itemForm.Price;
        item.OriginalPrice = itemForm.OriginalPrice;
        item.Active = itemForm.Active;
        if (itemForm.Id == 0) db.Catalog.Add(item);
        await db.SaveChangesAsync();

        if (item.Kind is "Product" or "Package")
        {
            if (itemForm.Id == 0)
            {
                var readyBike = item.Kind == "Package";
                var sizes = readyBike ? new[] { "" } : item.Category == "helmet" ? new[] { "S", "M", "L", "XL" } : new[] { "มาตรฐาน" };
                foreach (var size in sizes)
                {
                    db.Stock.Add(new StockItem
                    {
                        CatalogItemId = item.Id, ItemCode = readyBike ? $"PROMO-{item.Id}" : $"SKU-{item.Id}-{size}", Size = size,
                        ItemName = item.Name, Category = item.Category, Unit = readyBike ? StoreSaleRules.ReadyBikeUnit : "ชิ้น", MinRequired = 1
                    });
                }
                await db.SaveChangesAsync();
            }
            else
            {
                await db.Stock.Where(stock => stock.CatalogItemId == item.Id)
                    .ExecuteUpdateAsync(stock => stock.SetProperty(row => row.ItemName, item.Name)
                        .SetProperty(row => row.Category, item.Category));
            }
        }
        if (itemForm.PinOnHome is bool pin)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO shop_home_pins (CatalogItemId, IsPinned, PinnedAt)
                VALUES ({item.Id}, {pin}, UTC_TIMESTAMP(6))
                ON DUPLICATE KEY UPDATE
                    PinnedAt = CASE WHEN IsPinned = FALSE AND {pin} = TRUE THEN UTC_TIMESTAMP(6) ELSE PinnedAt END,
                    IsPinned = {pin}
                """);
        }

        await transaction.CommitAsync();
        TempData["StatusMessage"] = "บันทึกรายการแล้ว";
        return RedirectToAction(nameof(Index), new { section });
    }

    private async Task<IActionResult> ItemError(CatalogAdminSection section, CatalogItemAdminForm form)
    {
        var page = await BuildPage(section);
        page.ItemForm = form;
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return View(nameof(Index), page);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCoupon(CouponAdminForm couponForm)
    {
        if (ModelState.IsValid && couponForm.Category != "*" &&
            !await db.Catalog.AnyAsync(item => item.Kind == "Product" && item.Category == couponForm.Category))
            ModelState.AddModelError("CouponForm.Category", "กรุณาเลือกหมวดสินค้าที่มีอยู่");

        var code = couponForm.Code?.Trim().ToUpperInvariant() ?? "";
        if (ModelState.IsValid && await db.PackageCodes.AnyAsync(item => item.Code == code))
            ModelState.AddModelError("CouponForm.Code", "โค้ดนี้ใช้กับแพ็กเกจบริการอยู่แล้ว");
        if (!ModelState.IsValid)
        {
            var page = await BuildPage(CatalogAdminSection.Coupons);
            page.CouponForm = couponForm;
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return View(nameof(Index), page);
        }

        await db.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO shop_coupons (Code, Percent, Category, Active)
            VALUES ({code}, {couponForm.Percent}, {couponForm.Category}, {couponForm.Active})
            ON DUPLICATE KEY UPDATE Percent = {couponForm.Percent}, Category = {couponForm.Category}, Active = {couponForm.Active}
            """);
        TempData["StatusMessage"] = "บันทึกคูปองแล้ว";
        return RedirectToAction(nameof(Index), new { section = CatalogAdminSection.Coupons });
    }
}
