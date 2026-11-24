using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public static class HomeCatalogSelection
{
    private const int StudioBikeCount = 4;

    public static IQueryable<CatalogItem> Items(IQueryable<CatalogItem> catalog, IQueryable<HomePin> pins) =>
        from item in catalog
        join pin in pins on item.Id equals pin.CatalogItemId
        where item.Active && pin.IsPinned && (item.Kind == "Bike" || item.Kind == "Product" || item.Kind == "Package")
        orderby pin.PinnedAt descending, item.Id descending
        select item;

    public static List<HomeCatalogShelf> Shelves(IEnumerable<CatalogItem> orderedItems)
    {
        var byKind = orderedItems.ToLookup(item => item.Kind);
        var bikes = byKind["Bike"].ToList();
        var shelves = new List<HomeCatalogShelf>
        {
            new() { Key = "studio-bikes", Title = "รถคัสตอมจากสตูดิโอ", Items = bikes.Take(StudioBikeCount).ToList() },
            new() { Key = "parts", Title = "อะไหล่และหมวกกันน็อก", Items = byKind["Product"].ToList() },
            new() { Key = "promotions", Title = "โปรโมชัน", Items = byKind["Package"].ToList() },
            new() { Key = "recommended-bikes", Title = "รถคัสตอมแนะนำ", Items = bikes.Skip(StudioBikeCount).ToList() }
        };
        return shelves.Where(shelf => shelf.Items.Count > 0).ToList();
    }
}
