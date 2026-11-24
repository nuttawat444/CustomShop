using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.ViewModels;
using Nuttawat_7324_L002.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.Controllers;

public class HomeController(Csi402dbContext db) : Controller
{
    [Authorize(Policy = ShopAccess.Storefront)]
    public async Task<IActionResult> Index()
    {
        var items = await HomeCatalogSelection.Items(db.Catalog.AsNoTracking(), db.HomePins.AsNoTracking()).ToListAsync();
        return View(new HomePageViewModel { Shelves = HomeCatalogSelection.Shelves(items) });
    }

    [Authorize(Policy = ShopAccess.CustomBikes)]
    public async Task<IActionResult> CustomBike() => View(await CatalogPageAsync("Bike"));

    [Authorize(Policy = ShopAccess.Products)]
    public async Task<IActionResult> Product() => View(await CatalogPageAsync("Product"));

    [Authorize(Policy = ShopAccess.Storefront)]
    public async Task<IActionResult> Promotion() => View(await CatalogPageAsync("Package"));

    private async Task<CatalogPage> CatalogPageAsync(string kind)
    {
        var catalog = db.Catalog.AsNoTracking();
        var items = await catalog.Where(item => item.Active && item.Kind == kind).OrderBy(item => item.Id).ToListAsync();
        var stock = kind is "Product" or "Package"
            ? await (from sku in db.Stock.AsNoTracking()
                     join item in catalog on sku.CatalogItemId equals item.Id
                     where item.Active && item.Kind == kind
                     orderby sku.Id
                     select sku).ToListAsync()
            : new List<StockItem>();
        var coupons = kind == "Package" ? await db.Coupons.AsNoTracking().Where(x => x.Active).OrderBy(x => x.Code).ToListAsync() : new List<Coupon>();
        var codes = kind == "Package" ? await db.PackageCodes.ToDictionaryAsync(x => x.CatalogItemId, x => x.Code) : new Dictionary<int, string>();
        return new CatalogPage(items, stock.ToLookup(x => x.CatalogItemId), coupons, codes)
        {
            BuildableBikeIds = kind == "Bike"
                ? (await db.BikePartSlots.Select(slot => slot.BikeId).Distinct().ToListAsync()).ToHashSet()
                : []
        };
    }

    [Authorize(Policy = ShopAccess.Storefront)]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
