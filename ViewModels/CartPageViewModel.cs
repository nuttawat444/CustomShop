using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Services;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class CartPageViewModel
{
    public required Basket Basket { get; set; }
    public CartLineViewModel CartLineForm { get; set; } = new();
    public CartReviewViewModel ReviewForm { get; set; } = new();
}

public sealed class CartLineViewModel
{
    [Range(1, int.MaxValue)]
    public int StockId { get; set; }

    [Range(0, 999)]
    public int Quantity { get; set; }

    public bool Increment { get; set; }
}

public sealed class CartReviewViewModel
{
    [Required]
    public string Revision { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(pickup|delivery)$")]
    public string Fulfillment { get; set; } = "pickup";

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(40)]
    public string? Code { get; set; }
}
