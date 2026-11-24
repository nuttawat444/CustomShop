using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class WarehouseListingViewModel : IValidatableObject
{
    public const decimal MaxPrice = 999_999_999m;

    public bool PublishNow { get; set; }
    public decimal? Price { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PublishNow && Price is null or <= 0)
        {
            yield return new ValidationResult("เปิดขายทันทีต้องระบุราคาขายมากกว่า 0 บาท", [nameof(Price)]);
        }
        else if (Price is decimal price && (price < 0 || price > MaxPrice || decimal.Round(price, 2) != price))
        {
            yield return new ValidationResult("ราคาขายต้องอยู่ระหว่าง 0 ถึง 999,999,999 บาทและมีทศนิยมไม่เกิน 2 ตำแหน่ง", [nameof(Price)]);
        }
    }
}

public sealed class WarehouseExistingListingViewModel
{
    [Range(1, int.MaxValue)]
    public int StockId { get; set; }

    public WarehouseListingViewModel Listing { get; set; } = new();
}
