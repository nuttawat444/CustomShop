using System.Data;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public sealed class BikeBuildService(Csi402dbContext db)
{
    public async Task<BikeBuildViewModel?> Configure(int bikeId)
    {
        var bike = await db.Catalog.AsNoTracking().SingleOrDefaultAsync(item => item.Id == bikeId && item.Active && item.Kind == "Bike");
        if (bike is null) return null;
        var slots = await db.BikePartSlots.AsNoTracking().Where(slot => slot.BikeId == bikeId).OrderBy(slot => slot.SortOrder).ToListAsync();
        if (slots.Count == 0) return null;
        var options = await (from allowed in db.BikePartOptions
            join stock in db.Stock on allowed.StockItemId equals stock.Id
            join product in db.Catalog on stock.CatalogItemId equals product.Id
            where allowed.BikeId == bikeId && product.Active && product.Kind == "Product"
            orderby product.Name, stock.Id
            select new BikeBuildOptionViewModel
            {
                SlotCode = allowed.SlotCode, StockItemId = stock.Id, PartName = product.Name,
                Sku = stock.ItemCode, Variant = stock.Size, Image = product.Image, UnitPrice = product.Price
            }).ToListAsync();
        var bySlot = options.ToLookup(option => option.SlotCode);
        var groups = slots.Select(slot => new BikeBuildSlotViewModel { Slot = slot, Options = bySlot[slot.SlotCode].ToList() }).ToList();
        return new BikeBuildViewModel
        {
            Bike = bike, Slots = groups,
            Form = new BikeQuoteForm { BikeId = bikeId, RequestId = Guid.NewGuid(), Lines = groups.Select(BikeBuildRules.Default).ToList() }
        };
    }

    public async Task RequestQuote(int buyerId, string name, BikeQuoteForm form)
    {
        if (form.RequestId == Guid.Empty) throw new ShopException("กรุณาเปิดหน้าปรับแต่งใหม่แล้วส่งคำขออีกครั้ง");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        await db.Database.SqlQuery<int>($"SELECT id AS Value FROM users WHERE id = {buyerId} FOR UPDATE").ToListAsync();

        var projectCode = $"PRJ-{form.RequestId:N}";
        var existing = await db.Projects.AsNoTracking().SingleOrDefaultAsync(project => project.ProjectCode == projectCode);
        if (existing is not null)
        {
            if (existing.UserId != buyerId || existing.CatalogItemId != form.BikeId)
                throw new ShopException("เลขอ้างอิงคำขอไม่ตรงกับรุ่นรถหรือบัญชีผู้ใช้");
            return;
        }

        var page = await Configure(form.BikeId) ?? throw new ShopException("ไม่พบรุ่นรถที่เปิดให้ปรับแต่ง");
        var parts = BikeBuildRules.Snapshot(page.Slots, form.Lines, projectCode);
        db.Projects.Add(new Project
        {
            ProjectCode = projectCode, UserId = buyerId, CatalogItemId = page.Bike.Id,
            ProjectName = page.Bike.Name, CustomerName = name,
            Status = QuoteWorkflow.Pending, Budget = page.Bike.Price
        });
        await db.SaveChangesAsync();
        db.ProjectParts.AddRange(parts);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
