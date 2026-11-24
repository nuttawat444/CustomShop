using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.ViewModels;

public enum CatalogAdminSection { CustomBike, Product, Promotion, Coupons }

public static class CatalogAdminSections
{
    public static string? Kind(CatalogAdminSection section) => section switch
    {
        CatalogAdminSection.CustomBike => "Bike",
        CatalogAdminSection.Product => "Product",
        CatalogAdminSection.Promotion => "Package",
        _ => null
    };

    public static CatalogAdminSection ForKind(string kind) => kind switch
    {
        "Bike" => CatalogAdminSection.CustomBike,
        "Package" => CatalogAdminSection.Promotion,
        _ => CatalogAdminSection.Product
    };

    public static string Label(CatalogAdminSection section) => section switch
    {
        CatalogAdminSection.CustomBike => "Custom Bike",
        CatalogAdminSection.Product => "Product",
        CatalogAdminSection.Promotion => "Promotion",
        _ => "คูปองส่วนลด"
    };
}

public sealed class CatalogAdminViewModel
{
    public CatalogAdminSection Section { get; set; }
    public HashSet<int> PinnedItemIds { get; set; } = [];
    public IReadOnlyList<CatalogItem> Items { get; set; } = [];
    public IReadOnlyList<Coupon> Coupons { get; set; } = [];
    public IReadOnlyList<string> Images { get; set; } = [];
    public IReadOnlyList<string> ProductCategories { get; set; } = [];
    public CatalogItemAdminForm? ItemForm { get; set; }
    public CouponAdminForm? CouponForm { get; set; }
}

public sealed class CatalogItemAdminForm : IValidatableObject
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required, RegularExpression("^(Product|Bike|Package)$")]
    public string Kind { get; set; } = "Product";

    [Required, StringLength(220)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Image { get; set; } = "hero.jpg";

    [StringLength(5000)]
    public string? Description { get; set; }

    [StringLength(5000)]
    public string? Features { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal Price { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal OriginalPrice { get; set; }

    public bool Active { get; set; }
    public bool? PinOnHome { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((Kind is "Product" or "Package") && Active && Price <= 0)
            yield return new ValidationResult("กรุณาตั้งราคาขายมากกว่า 0 บาทก่อนเปิดแสดงสินค้า", [nameof(Price)]);
        if (decimal.Round(Price, 2) != Price || decimal.Round(OriginalPrice, 2) != OriginalPrice)
            yield return new ValidationResult("ราคาใช้ทศนิยมได้ไม่เกิน 2 ตำแหน่ง", [nameof(Price), nameof(OriginalPrice)]);
    }

    public static CatalogItemAdminForm FromItem(CatalogItem item) => new()
    {
        Id = item.Id, Kind = item.Kind, Name = item.Name, Category = item.Category, Image = item.Image,
        Description = item.Description, Features = item.Features, Price = item.Price, OriginalPrice = item.OriginalPrice,
        Active = item.Active
    };
}

public sealed class CouponAdminForm : IValidatableObject
{
    [Required, StringLength(40)]
    public string Code { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100")]
    public decimal Percent { get; set; }

    [Required, StringLength(100)]
    public string Category { get; set; } = "*";

    public bool Active { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (decimal.Round(Percent, 2) != Percent)
            yield return new ValidationResult("เปอร์เซ็นต์ส่วนลดใช้ทศนิยมได้ไม่เกิน 2 ตำแหน่ง", [nameof(Percent)]);
    }
}
