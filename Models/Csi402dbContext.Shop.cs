using Microsoft.EntityFrameworkCore;

namespace Nuttawat_7324_L002.Models;

public partial class Csi402dbContext
{
    internal async Task<Project?> LockQuoteProject(string code)
    {
        var projects = await Projects.FromSqlInterpolated($"SELECT * FROM shop_projects WHERE ProjectCode = {code} FOR UPDATE").ToListAsync();
        return projects.SingleOrDefault();
    }

    public DbSet<BikePartSlot> BikePartSlots => Set<BikePartSlot>();
    public DbSet<BikePartOption> BikePartOptions => Set<BikePartOption>();
    public DbSet<ProjectPart> ProjectParts => Set<ProjectPart>();
    public DbSet<QuoteEstimate> QuoteEstimates => Set<QuoteEstimate>();
    public DbSet<QuoteEstimateLine> QuoteEstimateLines => Set<QuoteEstimateLine>();
    public DbSet<PackageCode> PackageCodes => Set<PackageCode>();
    public DbSet<SavedOffer> SavedOffers => Set<SavedOffer>();
    public DbSet<HomePin> HomePins => Set<HomePin>();
    public DbSet<CatalogItem> Catalog => Set<CatalogItem>();
    public DbSet<StockItem> Stock => Set<StockItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartLine> CartLines => Set<CartLine>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<SaleOrder> Orders => Set<SaleOrder>();
    public DbSet<SaleLine> OrderLines => Set<SaleLine>();
    public DbSet<PaymentAttempt> Payments => Set<PaymentAttempt>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<WarehouseTask> WarehouseTasks => Set<WarehouseTask>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<JobAssignee> JobAssignees => Set<JobAssignee>();
    public DbSet<MechanicJob> Jobs => Set<MechanicJob>();
}
