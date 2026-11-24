using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class CustomerReceiptTests
{
    [Fact]
    public void ReceiptUsesPersistedInvoiceDetailsAndBangkokTime()
    {
        var order = new SaleOrder { Id = 17, Total = 129.50m };
        var paidAt = new DateTime(2026, 11, 23, 20, 30, 0, DateTimeKind.Utc);
        var invoice = new Invoice { InvoiceNo = "SHOP-17", PaymentMethod = "QR จำลอง", PaidAt = paidAt };

        var receipt = new CustomerReceiptViewModel { Order = order, Invoice = invoice, Lines = [] };

        Assert.Equal("SHOP-17", receipt.ReceiptNumber);
        Assert.Equal("QR จำลอง", receipt.PaymentLabel);
        Assert.Equal(new DateTime(2026, 11, 24, 3, 30, 0), receipt.PaidInBangkok);
        Assert.Equal(129.50m, receipt.Order.Total);
    }

    [Fact]
    public void LegacyReceiptCanDisplayItsOrderReference()
    {
        var order = new SaleOrder { Id = 9, CreatedAt = new DateTime(2026, 11, 23, 12, 0, 0, DateTimeKind.Utc) };

        var receipt = new CustomerReceiptViewModel { Order = order, Lines = [] };

        Assert.Equal("SHOP-9", receipt.ReceiptNumber);
        Assert.Equal(19, receipt.PaidInBangkok.Hour);
    }
}
