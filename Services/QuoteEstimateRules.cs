using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public static class QuoteEstimateRules
{
    public const decimal MaximumAmount = 10_000_000m;

    public static decimal Total(
        IReadOnlyList<(int Quantity, decimal UnitPrice)> lines, decimal labor, decimal other, decimal discount)
    {
        if (!IsMoney(labor) || !IsMoney(other) || !IsMoney(discount) ||
            lines.Any(line => line.Quantity <= 0 || !IsMoney(line.UnitPrice)))
            throw new ShopException("จำนวนต้องมากกว่าศูนย์ และราคาต้องอยู่ในช่วงที่กำหนดและมีทศนิยมไม่เกิน 2 ตำแหน่ง");

        var subtotal = lines.Sum(line => line.Quantity * line.UnitPrice) + labor + other;
        if (discount > subtotal || subtotal - discount > MaximumAmount)
            throw new ShopException("ส่วนลดต้องไม่เกินยอดก่อนลด และยอดสุทธิต้องไม่เกิน 10,000,000 บาท");

        return subtotal - discount;
    }

    public static bool MatchesVersion(QuoteEstimate? estimate, int revision, int version) =>
        (estimate?.Revision ?? 0) == revision && (estimate?.EditVersion ?? 0) == version;

    public static bool IsAwaitingReview(QuoteEstimate? estimate, int revision) =>
        estimate is { SubmittedAt: not null, ReviewedAt: null } && estimate.Revision == revision;

    public static Dictionary<string, decimal> PricesForParts(
        IReadOnlyList<ProjectPart> parts, IReadOnlyList<QuoteEstimatePriceForm> inputs)
    {
        var expected = parts.Select(part => part.SlotCode).ToHashSet(StringComparer.Ordinal);
        var prices = new Dictionary<string, decimal>(StringComparer.Ordinal);
        if (inputs.Count != expected.Count ||
            inputs.Any(input => !expected.Contains(input.SlotCode) || !prices.TryAdd(input.SlotCode, input.UnitPrice)))
            throw new ShopException("รายการราคาไม่ตรงกับอะไหล่ในคำขอ กรุณาเปิดใบประเมินใหม่");
        return prices;
    }

    private static bool IsMoney(decimal value) =>
        value is >= 0 and <= MaximumAmount && value == decimal.Round(value, 2);
}
