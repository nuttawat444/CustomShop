using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.Services;

public static class StoreSaleRules
{
    public const string ReadyBikeUnit = "คัน";

    public static IQueryable<int> AvailableStockIds(IQueryable<CatalogItem> catalog, IQueryable<StockItem> stock) =>
        from item in catalog
        join sku in stock on item.Id equals sku.CatalogItemId
        where item.Active && item.Price > 0 && (item.Kind == "Product" || (item.Kind == "Package" && sku.Unit == ReadyBikeUnit && sku.Size == ""))
        select sku.Id;

    public static bool CanUseProductCoupon(BasketRow row, string category) =>
        row.Kind == "Product" && (category == "*" || row.Category == category);
}
