using Nuttawat_7324_L002.Models;
using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class AccountingDashboardViewModel
{
    public decimal TodayRevenue { get; set; }
    public decimal MonthToDateRevenue { get; set; }
    public decimal OutstandingInvoices { get; set; }
    public int TotalCouponsRedeemed { get; set; }
    public List<Invoice> Invoices { get; set; } = new();
    public AccountingCreateInvoiceViewModel NewInvoiceForm { get; set; } = new();
}


public class AccountingCreateInvoiceViewModel
{
    [Required(ErrorMessage = "กรุณากรอกชื่อลูกค้า")]
    [Display(Name = "ชื่อลูกค้า / ผู้สั่งซื้อ")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณาระบุรายละเอียด")]
    [Display(Name = "รายการสินค้าหรือบริการ")]
    public string ItemDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกยอดเงิน")]
    [Range(1, 10000000, ErrorMessage = "ยอดเงินต้องมากกว่า 0 บาท")]
    [Display(Name = "ยอดรวม (บาท)")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "ส่วนลดคูปอง / โปรโมชั่น (บาท)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Required(ErrorMessage = "กรุณาเลือกช่องทางชำระ")]
    [Display(Name = "ช่องทางการชำระ")]
    public string PaymentMethod { get; set; } = "โอนผ่าน QR ธนาคาร";
}
