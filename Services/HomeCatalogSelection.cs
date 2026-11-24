using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.Services;

public static class HomeCatalogSelection
{
    public static IQueryable<CatalogItem> Items(IQueryable<CatalogItem> catalog, IQueryable<HomePin> pins) =>
        from item in catalog
        join pin in pins on item.Id equals pin.CatalogItemId
        where item.Active && pin.IsPinned && (item.Kind == "Bike" || item.Kind == "Product" || item.Kind == "Package")
        orderby pin.PinnedAt descending, item.Id descending
        select item;
}
