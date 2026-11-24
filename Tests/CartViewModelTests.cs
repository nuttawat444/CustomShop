using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class CartViewModelTests
{
    [Theory]
    [InlineData(1, 0, true)]
    [InlineData(1, 999, true)]
    [InlineData(1, 1000, false)]
    [InlineData(1, -1, false)]
    [InlineData(0, 1, false)]
    public void CartQuantityAcceptsRemovalAndValidAmounts(int stock, int quantity, bool expected)
    {
        var form = new CartLineViewModel { StockId = stock, Quantity = quantity };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), null, true);

        Assert.Equal(expected, valid);
    }

    [Theory]
    [InlineData("pickup", 0, 0, true)]
    [InlineData("delivery", 500, 40, true)]
    [InlineData("delivery", 501, 40, false)]
    [InlineData("pickup", 0, 41, false)]
    [InlineData("other", 0, 0, false)]
    public void CartReviewLimitsFieldsBeforeCalculatingPrice(string fulfillment, int addressLength, int codeLength, bool expected)
    {
        var form = new CartReviewViewModel
        {
            Revision = "active-cart",
            Fulfillment = fulfillment,
            Address = new string('a', addressLength),
            Code = new string('c', codeLength)
        };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), null, true);

        Assert.Equal(expected, valid);
    }
}
