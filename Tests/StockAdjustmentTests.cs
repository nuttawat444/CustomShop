using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class StockAdjustmentTests
{
    [Theory]
    [InlineData(1, 1, "Restock", true)]
    [InlineData(1, 1, "Dispatch", true)]
    [InlineData(2, 10_000, "Restock", true)]
    [InlineData(0, 1, "Restock", false)]
    [InlineData(1, 0, "Dispatch", false)]
    [InlineData(1, -1, "Dispatch", false)]
    [InlineData(1, 10_001, "Restock", false)]
    [InlineData(1, 1, "Set", false)]
    public void AdjustStockChecksInputBeforeDatabaseUpdate(int id, int amount, string action, bool expected)
    {
        var form = new StockAdjustmentViewModel { Id = id, Amount = amount, ActionType = action };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), new List<ValidationResult>(), true);

        Assert.Equal(expected, valid);
    }
}
