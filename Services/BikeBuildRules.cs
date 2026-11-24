using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public static class BikeBuildRules
{
    public static BikeQuoteLineForm Default(BikeBuildSlotViewModel group)
    {
        var slot = group.Slot;
        var option = group.Options.FirstOrDefault(item => item.StockItemId == slot.DefaultStockItemId);
        if (option is null && slot.MinQuantity > 0) option = group.Options.FirstOrDefault();
        return new BikeQuoteLineForm { SlotCode = slot.SlotCode, StockItemId = option?.StockItemId, Quantity = slot.MinQuantity };
    }

    public static IReadOnlyList<ProjectPart> Snapshot(IReadOnlyList<BikeBuildSlotViewModel> groups,
        IReadOnlyList<BikeQuoteLineForm> choices, string projectCode)
    {
        var bySlot = choices.ToLookup(choice => choice.SlotCode, StringComparer.Ordinal);
        if (groups.Count == 0 || groups.Count != choices.Count || bySlot.Count != choices.Count)
            throw new ShopException("กรุณาตรวจสอบรายการอะไหล่ทุกรายการ");

        var parts = new List<ProjectPart>();
        foreach (var group in groups)
        {
            var slot = group.Slot;
            var choice = bySlot[slot.SlotCode].SingleOrDefault();
            if (choice is null || choice.Quantity < slot.MinQuantity || choice.Quantity > slot.MaxQuantity)
                throw new ShopException($"จำนวนอะไหล่ไม่ถูกต้องสำหรับ {slot.SlotName}");

            var option = group.Options.SingleOrDefault(item => item.StockItemId == choice.StockItemId);
            if ((choice.Quantity > 0 || choice.StockItemId.HasValue) && option is null)
                throw new ShopException($"กรุณาเลือกอะไหล่ที่ใช้กับ {slot.SlotName} ของรถรุ่นนี้");

            if (choice.Quantity == 0) continue;
            parts.Add(new ProjectPart
            {
                ProjectCode = projectCode, SlotCode = slot.SlotCode, SlotName = slot.SlotName,
                StockItemId = option!.StockItemId, Sku = option.Sku, PartName = option.PartName,
                Variant = option.Variant, UnitPrice = option.UnitPrice, Quantity = choice.Quantity
            });
        }
        return parts;
    }
}
