using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.ViewModels;

public sealed record OrderReceipt(SaleOrder Order, List<SaleLine> Lines);
