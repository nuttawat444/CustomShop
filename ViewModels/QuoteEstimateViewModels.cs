using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class QuoteEstimateForm
{
    [Required, StringLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    [BindRequired, Range(0, int.MaxValue)]
    public int Revision { get; set; }
    [BindRequired, Range(0, int.MaxValue)]
    public int EditVersion { get; set; }
    [BindRequired, Display(Name = "ค่าแรง (บาท)")]
    public decimal LaborAmount { get; set; }
    [BindRequired, Display(Name = "ค่าตัวรถหรืองานเพิ่มเติม (บาท)")]
    public decimal OtherAmount { get; set; }
    [BindRequired, Display(Name = "ส่วนลด (บาท)")]
    public decimal DiscountAmount { get; set; }
    [Display(Name = "หมายเหตุ"), StringLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public List<QuoteEstimatePriceForm> Lines { get; set; } = [];
}

public sealed class QuoteEstimatePriceForm
{
    [Required, StringLength(40)]
    public string SlotCode { get; set; } = string.Empty;
    [BindRequired]
    public decimal UnitPrice { get; set; }
}

public sealed class QuoteEstimateQueueRowViewModel
{
    public required Project Project { get; set; }
    public int? LatestRevision { get; set; }
}

public sealed class QuoteEstimateReturnForm
{
    [Required, StringLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int Revision { get; set; }
    [Required, StringLength(1000)]
    public string Reason { get; set; } = string.Empty;
}

public sealed class QuoteEstimateEditorViewModel
{
    public required Project Project { get; set; }
    public List<ProjectPart> Parts { get; set; } = [];
    public List<QuoteEstimate> History { get; set; } = [];
    public QuoteEstimateForm Form { get; set; } = new();
}

public sealed class QuoteEstimateDocumentViewModel
{
    public required Project Project { get; set; }
    public required QuoteEstimate Estimate { get; set; }
    public List<QuoteEstimateLine> Lines { get; set; } = [];
}
