using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public enum WarehouseListingResult
{
    Added,
    DuplicateSku,
    StockNotFound,
    AlreadyLinked
}

public sealed class WarehouseCatalogService(Csi402dbContext db)
{
    public async Task<WarehouseListingResult> Add(WarehouseWorkerAddItemViewModel form, CancellationToken cancellationToken)
    {
        var stock = new StockItem
        {
            ItemCode = form.ItemCode.Trim().ToUpperInvariant(),
            ItemName = form.ItemName.Trim(),
            Category = form.Category.Trim() == "หมวกกันน็อก" ? "helmet" : form.Category.Trim(),
            Size = form.Size?.Trim() ?? "",
            QuantityOnHand = form.QuantityOnHand,
            MinRequired = form.MinRequired,
            Unit = form.Unit.Trim(),
            Location = form.Location.Trim()
        };
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.Stock.Add(stock);
        try
        {
            // Claim the unique SKU before creating the catalog record so a retry has no second listing.
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException { Number: 1062 })
        {
            return WarehouseListingResult.DuplicateSku;
        }
        await Link(stock, form.Listing, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return WarehouseListingResult.Added;
    }

    public async Task<WarehouseListingResult> ListExisting(WarehouseExistingListingViewModel form, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        // The row lock serializes simultaneous attempts to attach the same stock record.
        var candidates = await db.Stock.FromSqlInterpolated($"SELECT * FROM shop_stock WHERE Id = {form.StockId} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var stock = candidates.SingleOrDefault();
        if (stock is null) return WarehouseListingResult.StockNotFound;
        if (stock.CatalogItemId.HasValue) return WarehouseListingResult.AlreadyLinked;
        await Link(stock, form.Listing, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return WarehouseListingResult.Added;
    }

    private async Task Link(StockItem stock, WarehouseListingViewModel listing, CancellationToken cancellationToken)
    {
        var product = new CatalogItem
        {
            Slug = Guid.NewGuid().ToString("N"),
            Kind = "Product",
            Name = stock.ItemName,
            Category = stock.Category == "หมวกกันน็อก" ? "helmet" : stock.Category,
            Image = "hero.jpg",
            Price = listing.Price ?? 0,
            Active = listing.PublishNow
        };
        db.Catalog.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        stock.CatalogItemId = product.Id;
        if (string.IsNullOrWhiteSpace(stock.Size)) stock.Size = "มาตรฐาน";
        await db.SaveChangesAsync(cancellationToken);
    }
}
