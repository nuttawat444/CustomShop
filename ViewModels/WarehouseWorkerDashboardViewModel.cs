using Nuttawat_7324_L002.Models;
using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class WarehouseWorkerDashboardViewModel
{
    public int TotalStockItems { get; set; }
    public int LowStockAlerts { get; set; }
    public int PendingShipments { get; set; }
    public int CompletedTasksToday { get; set; }
    public List<StockItem> InventoryItems { get; set; } = new();
    public List<WarehouseTask> DailyTasks { get; set; } = new();
    public Dictionary<int, bool> ProductVisibility { get; set; } = new();
    public WarehouseExistingListingViewModel ExistingListingForm { get; set; } = new();
    public StockAdjustmentViewModel StockForm { get; set; } = new();
    public WarehouseWorkerAddItemViewModel NewItemForm { get; set; } = new();
}



public class WarehouseWorkerAddItemViewModel
{
    public WarehouseListingViewModel Listing { get; set; } = new();

    [StringLength(20)]
    public string? Size { get; set; }

    [Required(ErrorMessage = "กรุณากรอกรหัสสินค้า")]
    [StringLength(100)]
    [Display(Name = "รหัสสินค้า")]
    public string ItemCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกชื่อสินค้า")]
    [StringLength(220)]
    [Display(Name = "ชื่ออะไหล่ / ชิ้นส่วน")]
    public string ItemName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่")]
    [StringLength(100)]
    [Display(Name = "หมวดหมู่")]
    public string Category { get; set; } = "อะไหล่เครื่องยนต์";

    [Required(ErrorMessage = "กรุณากรอกจำนวนเริ่มต้น")]
    [Range(1, 10000, ErrorMessage = "จำนวนต้องมากกว่า 0")]
    [Display(Name = "จำนวนคงเหลือ")]
    public int QuantityOnHand { get; set; } = 5;

    [Required(ErrorMessage = "กรุณาระบุจำนวนขั้นต่ำ")]
    [Range(1, 1000, ErrorMessage = "ขั้นต่ำต้องมากกว่า 0")]
    [Display(Name = "จุดเตือนสั่งซื้อ")]
    public int MinRequired { get; set; } = 3;

    [Required(ErrorMessage = "กรุณาระบุหน่วยนับ")]
    [StringLength(30)]
    [Display(Name = "หน่วยนับ")]
    public string Unit { get; set; } = "ชิ้น";

    [Required(ErrorMessage = "กรุณาระบุชั้นวาง")]
    [StringLength(80)]
    [Display(Name = "ตำแหน่งจัดเก็บ")]
    public string Location { get; set; } = "A-01-01";
}

public sealed class StockAdjustmentViewModel
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Range(1, 10000)]
    public int Amount { get; set; } = 1;

    [Required]
    [RegularExpression("^(Restock|Dispatch)$")]
    public string ActionType { get; set; } = string.Empty;
}
