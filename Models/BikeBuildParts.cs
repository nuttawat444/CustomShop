using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nuttawat_7324_L002.Models;

[Table("shop_bike_part_slots")]
[PrimaryKey(nameof(BikeId), nameof(SlotCode))]
public sealed class BikePartSlot
{
    public int BikeId { get; set; }
    [MaxLength(40)]
    public string SlotCode { get; set; } = string.Empty;
    [MaxLength(120)]
    public string SlotName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int MinQuantity { get; set; }
    public int MaxQuantity { get; set; }
    public int? DefaultStockItemId { get; set; }
}

[Table("shop_bike_part_options")]
[PrimaryKey(nameof(BikeId), nameof(SlotCode), nameof(StockItemId))]
public sealed class BikePartOption
{
    public int BikeId { get; set; }
    [MaxLength(40)]
    public string SlotCode { get; set; } = string.Empty;
    public int StockItemId { get; set; }
}

[Table("shop_project_parts")]
[PrimaryKey(nameof(ProjectCode), nameof(SlotCode))]
public sealed class ProjectPart
{
    [MaxLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    [MaxLength(40)]
    public string SlotCode { get; set; } = string.Empty;
    [MaxLength(120)]
    public string SlotName { get; set; } = string.Empty;
    public int StockItemId { get; set; }
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    [MaxLength(220)]
    public string PartName { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Variant { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Precision(14, 2)]
    public decimal UnitPrice { get; set; }
}
