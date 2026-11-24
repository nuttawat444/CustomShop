using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class WarehouseListingTests
{
    [Theory]
    [InlineData(true, null, false)]
    [InlineData(true, "0", false)]
    [InlineData(true, "0.01", true)]
    [InlineData(true, "999999999", true)]
    [InlineData(true, "999999999.01", false)]
    [InlineData(true, "1.005", false)]
    [InlineData(false, null, true)]
    [InlineData(false, "0", true)]
    [InlineData(false, "125.50", true)]
    [InlineData(false, "-1", false)]
    public void ValidateAppliesPriceRulesForTheSelectedListingOption(bool publishNow, string? amount, bool expected)
    {
        var listing = new WarehouseListingViewModel
        {
            PublishNow = publishNow,
            Price = amount is null ? null : decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture)
        };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(listing, new ValidationContext(listing), errors, validateAllProperties: true);

        Assert.Equal(expected, valid);
        if (!expected) Assert.Contains(nameof(WarehouseListingViewModel.Price), Assert.Single(errors).MemberNames);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    public void ExistingListingRequiresAStockIdentifier(int stockId, bool expected)
    {
        var form = new WarehouseExistingListingViewModel { StockId = stockId };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), null, validateAllProperties: true);

        Assert.Equal(expected, valid);
    }
}
