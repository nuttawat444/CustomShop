using Nuttawat_7324_L002.Models;
namespace Nuttawat_7324_L002.ViewModels;

public sealed record CatalogPage(List<CatalogItem> Items, ILookup<int?, StockItem> Stock, List<Coupon> Coupons, Dictionary<int, string> PackageCodes)
{
    public HashSet<int> BuildableBikeIds { get; set; } = [];
    public CartLineViewModel CartLineForm { get; set; } = new() { Increment = true, Quantity = 1 };
}

public sealed record MemberOffer(string Code, string Description, int? PackageId);
