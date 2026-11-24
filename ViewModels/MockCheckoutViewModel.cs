using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Services;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class MockCheckoutViewModel
{
    public required CheckoutReview Review { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string QrImage { get; set; } = string.Empty;
    public MockCheckoutForm CheckoutForm { get; set; } = new();
}

public sealed class MockCheckoutForm
{
    [Required]
    public string Revision { get; set; } = string.Empty;

    [Required, RegularExpression("^(pickup|delivery)$")]
    public string Fulfillment { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(40)]
    public string? Code { get; set; }

    [Required, Range(typeof(decimal), "0", "999999999999.99")]
    public decimal? ExpectedTotal { get; set; }

    [Required, RegularExpression("^(success|failure)$")]
    public string Result { get; set; } = string.Empty;
}
