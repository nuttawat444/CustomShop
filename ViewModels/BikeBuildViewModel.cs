using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class BikeBuildViewModel
{
    public required CatalogItem Bike { get; set; }
    public List<BikeBuildSlotViewModel> Slots { get; set; } = [];
    public BikeQuoteForm Form { get; set; } = new();
}

public sealed class BikeBuildSlotViewModel
{
    public required BikePartSlot Slot { get; set; }
    public List<BikeBuildOptionViewModel> Options { get; set; } = [];
}

public sealed class BikeBuildOptionViewModel
{
    public string SlotCode { get; set; } = string.Empty;
    public int StockItemId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Label => $"{PartName} · {Variant} (฿{UnitPrice:N2})";
}

public sealed class BikeQuoteForm
{
    [Range(1, int.MaxValue)]
    public int BikeId { get; set; }
    public Guid RequestId { get; set; }
    public List<BikeQuoteLineForm> Lines { get; set; } = [];
}

public sealed class BikeQuoteLineForm
{
    [Required, StringLength(40)]
    public string SlotCode { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int? StockItemId { get; set; }
    [Range(0, 999)]
    public int Quantity { get; set; }
}

public sealed class QuoteHistoryViewModel
{
    public List<Project> Projects { get; set; } = [];
    public ILookup<string, ProjectPart> Parts { get; set; } = Array.Empty<ProjectPart>().ToLookup(part => part.ProjectCode);
}
