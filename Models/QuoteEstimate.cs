using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nuttawat_7324_L002.Models;

[Table("shop_quote_estimates")]
[PrimaryKey(nameof(ProjectCode), nameof(Revision))]
public sealed class QuoteEstimate
{
    [MaxLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    public int Revision { get; set; }
    public int EditVersion { get; set; }
    [Precision(14, 2)]
    public decimal LaborAmount { get; set; }
    [Precision(14, 2)]
    public decimal OtherAmount { get; set; }
    [Precision(14, 2)]
    public decimal DiscountAmount { get; set; }
    [Precision(14, 2)]
    public decimal TotalAmount { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public int UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    [MaxLength(1000)]
    public string? ReturnReason { get; set; }
}

[Table("shop_quote_estimate_lines")]
[PrimaryKey(nameof(ProjectCode), nameof(Revision), nameof(SlotCode))]
public sealed class QuoteEstimateLine
{
    [MaxLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    public int Revision { get; set; }
    [MaxLength(40)]
    public string SlotCode { get; set; } = string.Empty;
    [MaxLength(220)]
    public string PartName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Variant { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Precision(14, 2)]
    public decimal UnitPrice { get; set; }
}
