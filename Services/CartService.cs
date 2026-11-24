using System.Data;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.Services;

public sealed record BasketRow(int StockId, string Name, string Size, decimal Price, int Quantity, int Available, string Category, string Image = "", string Description = "", string Unit = "ชิ้น", string Kind = "Product");
public sealed record Basket(string Revision, List<BasketRow> Rows)
{
    public decimal Total => Rows.Sum(row => row.Quantity * row.Price);
}

public sealed class CartService(Csi402dbContext db)
{
    // Serialize changes for this buyer, including the first request before a cart exists.
    private async Task<Cart> LockCart(int buyer)
    {
        await db.Database.SqlQuery<int>($"SELECT id AS Value FROM users WHERE id = {buyer} FOR UPDATE").ToListAsync();
        var cart = await db.Carts.FindAsync(buyer);
        if (cart != null) return cart;
        cart = new Cart { UserId = buyer, Revision = Guid.NewGuid().ToString("N") };
        db.Carts.Add(cart);
        await db.SaveChangesAsync();
        return cart;
    }

    private IQueryable<int> SaleStock() => StoreSaleRules.AvailableStockIds(db.Catalog, db.Stock);

    private IQueryable<BasketRow> Rows(int buyer)
    {
        var availableStock = SaleStock();
        return
        from line in db.CartLines
        join stock in db.Stock on line.StockItemId equals stock.Id
        join item in db.Catalog on stock.CatalogItemId equals item.Id
        where line.UserId == buyer
        orderby stock.Id
        select new BasketRow(stock.Id, item.Name, stock.Size, item.Price, line.Quantity,
            availableStock.Contains(stock.Id) ? stock.QuantityOnHand : 0, item.Category, item.Image, item.Description, stock.Unit, item.Kind);
    }

    public async Task<Basket> Get(int buyer)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var cart = await LockCart(buyer);
        var basket = new Basket(cart.Revision, await Rows(buyer).ToListAsync());
        await tx.CommitAsync();
        return basket;
    }

    public async Task Change(int buyer, int stockId, int quantity, bool increment)
    {
        if (quantity < 0 || quantity > 999 || (increment && quantity == 0))
            throw new ShopException("จำนวนสินค้าไม่ถูกต้อง");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var cart = await LockCart(buyer);
        var line = await db.CartLines.SingleOrDefaultAsync(x => x.UserId == buyer && x.StockItemId == stockId);
        var desired = quantity + (increment ? line?.Quantity ?? 0 : 0);
        if (desired == 0)
        {
            if (line != null) db.CartLines.Remove(line);
        }
        else
        {
            var saleStock = SaleStock();
            var available = await db.Stock.Where(stock => stock.Id == stockId && saleStock.Contains(stock.Id))
                .Select(stock => stock.QuantityOnHand).SingleOrDefaultAsync();
            if (desired > available || desired > 999) throw new ShopException("จำนวนสินค้าในคลังไม่เพียงพอ");
            if (line == null) db.CartLines.Add(new CartLine { UserId = buyer, StockItemId = stockId, Quantity = desired });
            else line.Quantity = desired;
        }
        cart.Revision = Guid.NewGuid().ToString("N");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private async Task<CartPrice> Price(List<BasketRow> rows, string? code)
    {
        var total = rows.Sum(x => x.Quantity * x.Price);
        decimal discount = 0;
        var couponCode = code?.Trim() ?? "";
        if (couponCode.Length > 0)
        {
            var coupon = await db.Coupons.SingleOrDefaultAsync(x => x.Code == couponCode && x.Active);
            if (coupon == null) throw new ShopException("โค้ดส่วนลดไม่สามารถใช้งานได้");
            var eligible = rows.Where(row => StoreSaleRules.CanUseProductCoupon(row, coupon.Category));
            discount = Math.Round(eligible.Sum(x => x.Price * x.Quantity) * coupon.Percent / 100, 2, MidpointRounding.AwayFromZero);
            if (discount <= 0) throw new ShopException("โค้ดนี้ใช้ไม่ได้กับสินค้าในตะกร้า");
            couponCode = coupon.Code;
        }
        return new CartPrice(total, discount, couponCode);
    }

    public async Task<CheckoutReview> Review(int buyer, string revision, string fulfillment, string? address, string? code)
    {
        var basket = await Get(buyer);
        if (basket.Revision != revision || basket.Rows.Count == 0 || basket.Rows.Any(row => row.Quantity > row.Available))
            throw new ShopException("กรุณาตรวจสอบสินค้าและจำนวนในตะกร้า");
        address = DeliveryAddress(fulfillment, address);
        return new CheckoutReview(basket, await Price(basket.Rows, code), fulfillment, address);
    }

    private static string DeliveryAddress(string fulfillment, string? address)
    {
        address = fulfillment == "delivery" ? address?.Trim() : "";
        if (fulfillment is not ("pickup" or "delivery") || address?.Length > 500 ||
            (fulfillment == "delivery" && string.IsNullOrWhiteSpace(address)))
            throw new ShopException("กรุณาเลือกวิธีรับสินค้าและระบุที่อยู่จัดส่ง");
        return address ?? "";
    }

    public async Task<int?> Checkout(int buyer, string name, string revision, string fulfillment,
        string? address, string? code, decimal expectedTotal, bool success)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var cart = await LockCart(buyer);
        var previous = await db.Orders.SingleOrDefaultAsync(x => x.UserId == buyer && x.Revision == revision);
        if (previous != null) return previous.Id;
        if (cart.Revision != revision) throw new ShopException("ตะกร้ามีการเปลี่ยนแปลง กรุณาตรวจสอบอีกครั้ง");
        address = DeliveryAddress(fulfillment, address);
        var rows = await Rows(buyer).ToListAsync();
        if (rows.Count == 0 || rows.Any(x => x.Available < x.Quantity))
            throw new ShopException("ตะกร้าว่างหรือสินค้าที่เลือกมีจำนวนไม่พอ");
        var quote = await Price(rows, code);
        if (quote.Total != expectedTotal)
            throw new ShopException("ราคาหรือส่วนลดเปลี่ยนแปลง กรุณาตรวจสอบยอดอีกครั้ง");
        var (total, discount, couponCode) = quote;
        var now = DateTime.UtcNow;
        var payment = new PaymentAttempt { UserId = buyer, CreatedAt = now, Amount = total - discount, Success = success };
        if (!success)
        {
            db.Payments.Add(payment);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return null;
        }
        var order = new SaleOrder { UserId = buyer, Revision = revision, CreatedAt = now, Subtotal = total,
            Discount = discount, CouponCode = couponCode, Total = payment.Amount, Address = address ?? "", Fulfillment = fulfillment };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        foreach (var row in rows)
        {
            var changed = await db.Stock.Where(x => x.Id == row.StockId && x.QuantityOnHand >= row.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.QuantityOnHand, x => x.QuantityOnHand - row.Quantity));
            if (changed != 1) throw new ShopException("สินค้าหมดระหว่างชำระเงิน กรุณาตรวจสอบตะกร้า");
            db.OrderLines.Add(new SaleLine { SaleOrderId = order.Id, StockItemId = row.StockId, Name = row.Name,
                Size = row.Kind == "Package" ? StoreSaleRules.ReadyBikeUnit : row.Size, UnitPrice = row.Price, Quantity = row.Quantity });
        }
        payment.SaleOrderId = order.Id;
        db.Payments.Add(payment);
        db.Invoices.Add(new Invoice { InvoiceNo = $"SHOP-{order.Id}", SaleOrderId = order.Id, CustomerName = name,
            ItemDescription = $"คำสั่งซื้อ #{order.Id}", TotalAmount = order.Total, DiscountAmount = discount,
            CreatedAt = now, PaidAt = now, Status = "ชำระเงินแล้ว", PaymentMethod = "QR จำลอง" });
        db.WarehouseTasks.Add(new WarehouseTask { SaleOrderId = order.Id,
            TaskTitle = fulfillment == "delivery" ? $"จัดส่ง #{order.Id} — {name} {address}" : $"เตรียมรับที่ร้าน #{order.Id} — {name}",
            DueTime = "หลังยืนยันชำระเงิน" });
        await db.CartLines.Where(x => x.UserId == buyer).ExecuteDeleteAsync();
        cart.Revision = Guid.NewGuid().ToString("N");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return order.Id;
    }
}

public sealed record CartPrice(decimal Subtotal, decimal Discount, string Code)
{
    public decimal Total => Subtotal - Discount;
}
public sealed record CheckoutReview(Basket Basket, CartPrice Price, string Fulfillment, string Address);

public sealed class ShopException(string message) : Exception(message);
