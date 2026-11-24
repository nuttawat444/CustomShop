using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;
using Xunit;

namespace CustomShop.Tests;

public class StoreSaleRulesTests
{
    [Fact]
    public void MemberCouponDiscountCountsOnlyEligibleProducts()
    {
        BasketRow[] rows =
        [
            new(1, "Helmet", "", 1000m, 2, 2, "helmet"),
            new(2, "Seat", "", 700m, 1, 1, "parts"),
            new(3, "Ready bike", "", 5000m, 1, 1, "helmet", Kind: "Package")
        ];

        Assert.Equal(240m, StoreSaleRules.ProductCouponDiscount(rows, "helmet", 12m));
        Assert.Equal(270m, StoreSaleRules.ProductCouponDiscount(rows, "*", 10m));
        Assert.Equal(0m, StoreSaleRules.ProductCouponDiscount(rows, "exhaust", 8m));
    }

    [Theory]
    [InlineData("SHOP10", true, false)]
    [InlineData("SHOP10", false, true)]
    [InlineData("LEGACY", true, true)]
    public void CouponReuseFollowsMemberCampaignRules(string code, bool previouslyUsed, bool expected)
    {
        Assert.Equal(expected, MemberCouponCampaign.CanRedeem(code, previouslyUsed));
    }

    [Fact]
    public void SaleStockIncludesReadyBikesAndPartsButExcludesHiddenOrUnpricedItemsAndLegacyPackages()
    {
        var catalog = new[]
        {
            new CatalogItem { Id = 1, Kind = "Product", Price = 50, Active = true },
            new CatalogItem { Id = 2, Kind = "Package", Price = 100, Active = true },
            new CatalogItem { Id = 3, Kind = "Package", Price = 100, Active = true },
            new CatalogItem { Id = 4, Kind = "Bike", Price = 100, Active = true },
            new CatalogItem { Id = 5, Kind = "Package", Price = 100, Active = false },
            new CatalogItem { Id = 6, Kind = "Package", Price = 0, Active = true }
        };
        var stock = catalog.Select(item => new StockItem
        {
            Id = item.Id + 10, CatalogItemId = item.Id, Unit = item.Id is 1 or 3 ? "ชิ้น" : "คัน"
        });

        var eligible = StoreSaleRules.AvailableStockIds(catalog.AsQueryable(), stock.AsQueryable()).ToArray();

        Assert.Equal(new[] { 11, 12 }, eligible);
    }

    [Theory]
    [InlineData("Product", "*", true)]
    [InlineData("Product", "parts", true)]
    [InlineData("Product", "helmet", false)]
    [InlineData("Package", "*", false)]
    public void ProductCouponKeepsItsExistingScope(string kind, string category, bool expected)
    {
        var row = new BasketRow(1, "รายการ", "", 100, 1, 1, "parts", Kind: kind);

        var eligible = StoreSaleRules.CanUseProductCoupon(row, category);

        Assert.Equal(expected, eligible);
    }
}
