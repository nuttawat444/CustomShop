using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Nuttawat_7324_L002.ViewModels;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;
using Xunit;

namespace CustomShop.Tests;

public class CatalogAdminTests
{
    [Theory]
    [InlineData(CatalogAdminSection.CustomBike, "Bike")]
    [InlineData(CatalogAdminSection.Product, "Product")]
    [InlineData(CatalogAdminSection.Promotion, "Package")]
    public void ShelfAndCatalogKindRoundTrip(CatalogAdminSection section, string expected)
    {
        var kind = CatalogAdminSections.Kind(section);

        Assert.Equal(expected, kind);
        Assert.Equal(section, CatalogAdminSections.ForKind(expected));
    }

    [Theory]
    [InlineData("Product", true, "0", false)]
    [InlineData("Product", false, "0", true)]
    [InlineData("Product", true, "0.01", true)]
    [InlineData("Product", true, "0.001", false)]
    [InlineData("Product", true, "999999999.01", false)]
    [InlineData("Bike", true, "0", true)]
    [InlineData("Package", true, "0", false)]
    [InlineData("Package", false, "0", true)]
    [InlineData("Unknown", false, "100", false)]
    public void ItemsValidateTypeAndPublicationPrice(string kind, bool active, string price, bool expected)
    {
        var form = new CatalogItemAdminForm
        {
            Kind = kind, Active = active, Price = decimal.Parse(price, CultureInfo.InvariantCulture), Name = "รายการ", Category = "parts"
        };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), null, true);

        Assert.Equal(expected, valid);
    }

    [Theory]
    [InlineData("0.01", true)]
    [InlineData("100", true)]
    [InlineData("0.001", false)]
    [InlineData("1.005", false)]
    [InlineData("100.01", false)]
    public void CouponsAcceptPercentInHundredths(string percent, bool expected)
    {
        var form = new CouponAdminForm { Code = "QA", Percent = decimal.Parse(percent, CultureInfo.InvariantCulture) };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), null, true);

        Assert.Equal(expected, valid);
    }

    [Fact]
    public void HomeOnlyShowsPublishedPinsIncludingHelmetsAndPromotionsInPinOrder()
    {
        var time = new DateTime(2026, 11, 23);
        var catalog = new[]
        {
            new CatalogItem { Id = 1, Kind = "Bike", Active = true },
            new CatalogItem { Id = 2, Kind = "Product", Category = "helmet", Active = true },
            new CatalogItem { Id = 3, Kind = "Package", Active = true },
            new CatalogItem { Id = 4, Kind = "Product", Active = false },
            new CatalogItem { Id = 5, Kind = "Bike", Active = true },
            new CatalogItem { Id = 6, Kind = "Bike", Active = true }
        };
        var pins = new[]
        {
            new HomePin { CatalogItemId = 1, IsPinned = true, PinnedAt = time },
            new HomePin { CatalogItemId = 2, IsPinned = true, PinnedAt = time.AddMinutes(1) },
            new HomePin { CatalogItemId = 3, IsPinned = true, PinnedAt = time.AddMinutes(1) },
            new HomePin { CatalogItemId = 4, IsPinned = true, PinnedAt = time },
            new HomePin { CatalogItemId = 5, IsPinned = false, PinnedAt = time }
        };

        var selected = HomeCatalogSelection.Items(catalog.AsQueryable(), pins.AsQueryable()).Select(item => item.Id).ToArray();

        Assert.Equal(new[] { 3, 2, 1 }, selected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(9)]
    public void HomeShelvesSplitBikesWithoutDroppingOrRepeatingPins(int bikeCount)
    {
        var bikes = Enumerable.Range(1, bikeCount).Reverse()
            .Select(id => new CatalogItem { Id = id, Kind = "Bike" }).ToArray();
        CatalogItem[] ordered = [new() { Id = 20, Kind = "Product" }, .. bikes, new() { Id = 21, Kind = "Package" }];

        var shelves = HomeCatalogSelection.Shelves(ordered);

        Assert.Equal(bikes.Select(item => item.Id),
            shelves.SelectMany(shelf => shelf.Items).Where(item => item.Kind == "Bike").Select(item => item.Id));
        Assert.Equal(Math.Min(bikeCount, 4), shelves.SingleOrDefault(shelf => shelf.Key == "studio-bikes")?.Items.Count ?? 0);
        Assert.Equal(Math.Max(0, bikeCount - 4), shelves.SingleOrDefault(shelf => shelf.Key == "recommended-bikes")?.Items.Count ?? 0);
        Assert.Equal(bikeCount > 4 ? "recommended-bikes" : "promotions", shelves.Last().Key);
        Assert.Equal(20, shelves.Single(shelf => shelf.Key == "parts").Items.Single().Id);
        Assert.Equal(21, shelves.Single(shelf => shelf.Key == "promotions").Items.Single().Id);
    }
}
