using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nuttawat_7324_L002.Models;

[Table("shop_catalog")]
public class CatalogItem
{
    [Key]
    public int Id { get; set; }
    [MaxLength(20)]
    public string Kind { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;
    [MaxLength(220)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    [MaxLength(120)]
    public string Image { get; set; } = string.Empty;
    [Precision(14, 2)]
    public decimal Price { get; set; }
    [Precision(14, 2)]
    public decimal OriginalPrice { get; set; }
    public bool Active { get; set; }
}

[Table("shop_stock")]
public class StockItem
{
    [NotMapped]
    public string StockStatus => QuantityOnHand <= MinRequired ? "Low" : "Normal";
    [Key]
    public int Id { get; set; }
    public int? CatalogItemId { get; set; }
    [MaxLength(100)]
    public string ItemCode { get; set; } = string.Empty;
    [MaxLength(220)]
    public string ItemName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Size { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public int MinRequired { get; set; }
    [MaxLength(30)]
    public string Unit { get; set; } = string.Empty;
    [MaxLength(80)]
    public string Location { get; set; } = string.Empty;
}

[Table("shop_carts")]
public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int UserId { get; set; }
    [MaxLength(32)]
    public string Revision { get; set; } = string.Empty;
}

[Table("shop_cart_lines")]
public class CartLine
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StockItemId { get; set; }
    public int Quantity { get; set; }
}

[Table("shop_coupons")]
public class Coupon
{
    [Key]
    public int Id { get; set; }
    [MaxLength(40)]
    public string Code { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    [Precision(14, 2)]
    public decimal Percent { get; set; }
    public bool Active { get; set; }
}

[Table("shop_orders")]
public class SaleOrder
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(32)]
    public string Revision { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    [MaxLength(20)]
    public string Fulfillment { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    [MaxLength(40)]
    public string CouponCode { get; set; } = string.Empty;
    [Precision(14, 2)]
    public decimal Subtotal { get; set; }
    [Precision(14, 2)]
    public decimal Discount { get; set; }
    [Precision(14, 2)]
    public decimal Total { get; set; }
}

[Table("shop_order_lines")]
public class SaleLine
{
    [Key]
    public int Id { get; set; }
    public int SaleOrderId { get; set; }
    public int StockItemId { get; set; }
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Size { get; set; } = string.Empty;
    [Precision(14, 2)]
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

[Table("shop_payments")]
public class PaymentAttempt
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? SaleOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    [Precision(14, 2)]
    public decimal Amount { get; set; }
    public bool Success { get; set; }
}

[Table("shop_invoices")]
public class Invoice
{
    [Key]
    [MaxLength(50)]
    public string InvoiceNo { get; set; } = string.Empty;
    public int? SaleOrderId { get; set; }
    [MaxLength(220)]
    public string CustomerName { get; set; } = string.Empty;
    [Precision(14, 2)]
    public decimal TotalAmount { get; set; }
    [Precision(14, 2)]
    public decimal DiscountAmount { get; set; }
    [MaxLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;
    [MaxLength(80)]
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
}

[Table("shop_warehouse_tasks")]
public class WarehouseTask
{
    [Key]
    public int Id { get; set; }
    public int? SaleOrderId { get; set; }
    [MaxLength(800)]
    public string TaskTitle { get; set; } = string.Empty;
    [MaxLength(80)]
    public string DueTime { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

[Table("shop_package_codes")]
public class PackageCode
{
    [Key, MaxLength(40)]
    public string Code { get; set; } = string.Empty;
    public int CatalogItemId { get; set; }
}

[Table("shop_saved_offers")]
public class SavedOffer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? CouponId { get; set; }
    public string? PackageCode { get; set; }
}

[Table("shop_projects")]
public class Project
{
    [Key]
    [MaxLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
    public int? CatalogItemId { get; set; }
    public int? UserId { get; set; }
    [MaxLength(220)]
    public string ProjectName { get; set; } = string.Empty;
    [MaxLength(220)]
    public string CustomerName { get; set; } = string.Empty;
    [Precision(14, 2)]
    public decimal Budget { get; set; }
    [MaxLength(100)]
    public string Status { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public bool IsApproved { get; set; }
}

[Table("shop_jobs")]
public class MechanicJob
{
    [Key]
    [MaxLength(50)]
    public string JobTicketNo { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? ProjectCode { get; set; }
    [MaxLength(220)]
    public string BikeModel { get; set; } = string.Empty;
    [MaxLength(100)]
    public string CustomStyle { get; set; } = string.Empty;
    [MaxLength(500)]
    public string ServiceType { get; set; } = string.Empty;
    [MaxLength(220)]
    public string AssignedMechanic { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Priority { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Status { get; set; } = string.Empty;
    public int ProgressPercent { get; set; }
    public DateTime? CompletedAt { get; set; }
}
