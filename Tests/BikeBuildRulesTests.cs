using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class BikeBuildRulesTests
{
    private static BikeBuildSlotViewModel Group(string code, int minimum, params int[] allowed) => new()
    {
        Slot = new BikePartSlot
        {
            SlotCode = code, SlotName = code, MinQuantity = minimum, MaxQuantity = 1,
            DefaultStockItemId = allowed.Length == 0 || minimum == 0 ? null : allowed[0]
        },
        Options = allowed.Select(id => new BikeBuildOptionViewModel
        {
            StockItemId = id, PartName = "Part " + id, Sku = "SKU-" + id, UnitPrice = 259
        }).ToList()
    };

    [Fact]
    public void DefaultsChooseRequiredPartAndLeaveAccessoryUnset()
    {
        var lamp = Group("lamp", 1, 10, 12);
        var rack = Group("rack", 0, 11);

        var lampChoice = BikeBuildRules.Default(lamp);
        var rackChoice = BikeBuildRules.Default(rack);

        Assert.Equal(10, lampChoice.StockItemId);
        Assert.Equal(1, lampChoice.Quantity);
        Assert.Null(rackChoice.StockItemId);
        Assert.Equal(0, rackChoice.Quantity);
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 2)]
    public void SnapshotSavesReplacementAndOnlyIncludesOptionalPartWhenAdded(bool addRack, int expectedCount)
    {
        var lamp = Group("lamp", 1, 10, 12);
        var rack = Group("rack", 0, 11);
        var choices = new[]
        {
            new BikeQuoteLineForm { SlotCode = "lamp", StockItemId = 12, Quantity = 1 },
            new BikeQuoteLineForm { SlotCode = "rack", StockItemId = 11, Quantity = addRack ? 1 : 0 }
        };

        var snapshot = BikeBuildRules.Snapshot([lamp, rack], choices, "PRJ-42");

        Assert.Equal(expectedCount, snapshot.Count);
        Assert.Equal("PRJ-42", snapshot[0].ProjectCode);
        Assert.Equal("SKU-12", snapshot[0].Sku);
        Assert.Equal(259, snapshot[0].UnitPrice);
    }

    [Theory]
    [InlineData("lamp", 10, 0)]
    [InlineData("lamp", 10, 2)]
    [InlineData("lamp", 999, 1)]
    [InlineData("lamp", null, 1)]
    [InlineData("other", 10, 1)]
    public void SnapshotRejectsRemovingRequiredPartIncompatibleSkuAndUnexpectedSlots(string slot, int? sku, int quantity)
    {
        var lamp = Group("lamp", 1, 10);
        var choices = new[] { new BikeQuoteLineForm { SlotCode = slot, StockItemId = sku, Quantity = quantity } };

        Assert.Throws<ShopException>(() => BikeBuildRules.Snapshot([lamp], choices, "PRJ-1"));
    }

    [Fact]
    public void SnapshotRejectsRepeatedSlotEvenWhenRowCountMatches()
    {
        var lamp = Group("lamp", 1, 10);
        var rack = Group("rack", 0, 11);
        var choice = BikeBuildRules.Default(lamp);

        Assert.Throws<ShopException>(() => BikeBuildRules.Snapshot([lamp, rack], [choice, choice], "PRJ-1"));
    }
}
