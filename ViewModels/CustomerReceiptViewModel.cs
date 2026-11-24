using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class CustomerReceiptViewModel
{
    public required SaleOrder Order { get; set; }
    public required IReadOnlyList<SaleLine> Lines { get; set; }
    public Invoice? Invoice { get; set; }

    public string ReceiptNumber => Invoice?.InvoiceNo ?? $"SHOP-{Order.Id}";
    public string PaymentLabel => Invoice?.PaymentMethod ?? "ชำระเงินจำลอง";
    public DateTime PaidInBangkok => (Invoice?.PaidAt ?? Order.CreatedAt).AddHours(7);
}
