using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;

using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Policy = ShopAccess.Buyer)]
[AutoValidateAntiforgeryToken]
public class CartController(CartService cart, Csi402dbContext db, BikeBuildService builds) : Controller
{
    private int BuyerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var basket = await cart.Get(BuyerId);
        return View(new CartPageViewModel
        {
            Basket = basket,
            ReviewForm = new CartReviewViewModel { Revision = basket.Revision }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Change(CartLineViewModel cartLineForm)
    {
        if (!ModelState.IsValid) return BadRequest();
        try { await cart.Change(BuyerId, cartLineForm.StockId, cartLineForm.Quantity, cartLineForm.Increment); }
        catch (ShopException ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int stockId)
    {
        await cart.Change(BuyerId, stockId, 0, false);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Review(CartReviewViewModel reviewForm)
    {
        try
        {
            if (!ModelState.IsValid) throw new ShopException("กรุณาตรวจสอบวิธีรับสินค้า ที่อยู่ และโค้ดส่วนลด");
            var review = await cart.Review(BuyerId, reviewForm.Revision, reviewForm.Fulfillment, reviewForm.Address, reviewForm.Code);
            var reference = MockPaymentQr.Reference(review.Basket.Revision);
            return View(new MockCheckoutViewModel
            {
                Review = review,
                Reference = reference,
                QrImage = MockPaymentQr.ImageDataUrl(reference, review.Price.Total),
                CheckoutForm = new MockCheckoutForm
                {
                    Revision = review.Basket.Revision,
                    Fulfillment = review.Fulfillment,
                    Address = review.Address,
                    Code = review.Price.Code,
                    ExpectedTotal = review.Price.Total
                }
            });
        }
        catch (ShopException ex) { TempData["ErrorMessage"] = ex.Message; return RedirectToAction(nameof(Index)); }
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(MockCheckoutForm checkoutForm)
    {
        try
        {
            if (!ModelState.IsValid || checkoutForm.ExpectedTotal is not decimal total)
                throw new ShopException("กรุณาตรวจสอบยอดและเลือกผลการชำระเงินจำลอง");
            var id = await cart.Checkout(BuyerId, User.Identity!.Name!, checkoutForm.Revision, checkoutForm.Fulfillment,
                checkoutForm.Address, checkoutForm.Code, total, checkoutForm.Result == "success");
            if (id.HasValue) return RedirectToAction(nameof(Receipt), new { id });
            TempData["ErrorMessage"] = "การชำระเงินจำลองไม่สำเร็จ คุณสามารถลองอีกครั้งได้";
        }
        catch (ShopException ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> History() =>
        View(await db.Orders.AsNoTracking().Where(x => x.UserId == BuyerId).OrderByDescending(x => x.Id).ToListAsync());

    public async Task<IActionResult> Receipt(int id)
    {
        var order = await db.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.UserId == BuyerId);
        if (order == null) return NotFound();
        var lines = await db.OrderLines.AsNoTracking().Where(x => x.SaleOrderId == id).OrderBy(x => x.Id).ToListAsync();
        var invoice = await db.Invoices.AsNoTracking().SingleOrDefaultAsync(x => x.SaleOrderId == order.Id);
        return View(new CustomerReceiptViewModel { Order = order, Lines = lines, Invoice = invoice });
    }

    [HttpPost]
    public async Task<IActionResult> RequestQuote(int id)
    {
        var item = await db.Catalog.SingleOrDefaultAsync(x => x.Id == id && x.Active && x.Kind == "Bike");
        if (item == null) return NotFound();
        if (await db.BikePartSlots.AnyAsync(slot => slot.BikeId == id))
            return RedirectToAction(nameof(ConfigureBike), new { id });
        db.Projects.Add(new Project { ProjectCode = $"PRJ-{Guid.NewGuid():N}", UserId = BuyerId,
            CatalogItemId = item.Id, ProjectName = item.Name, CustomerName = User.Identity!.Name!,
            Status = QuoteWorkflow.Pending, Budget = item.Price });
        await db.SaveChangesAsync();
        TempData["StatusMessage"] = "ส่งคำขอให้เจ้าของร้านแล้ว";
        return RedirectToAction(nameof(Quotes));
    }

    [HttpGet]
    public async Task<IActionResult> ConfigureBike(int id)
    {
        var page = await builds.Configure(id);
        if (page is null) return NotFound();
        return View(page);
    }

    [HttpPost]
    public async Task<IActionResult> ConfigureBike(BikeQuoteForm form)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "กรุณาตรวจสอบรุ่นรถและรายการอะไหล่";
            return RedirectToAction(nameof(ConfigureBike), new { id = form.BikeId });
        }
        try
        {
            await builds.RequestQuote(BuyerId, User.Identity!.Name!, form);
            TempData["StatusMessage"] = "ส่งรายการปรับแต่งให้ Owner ประเมินราคาแล้ว";
            return RedirectToAction(nameof(Quotes));
        }
        catch (ShopException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(ConfigureBike), new { id = form.BikeId });
        }
    }

    public async Task<IActionResult> Quotes()
    {
        var projects = db.Projects.AsNoTracking().Where(project => project.UserId == BuyerId);
        var parts = await (from part in db.ProjectParts.AsNoTracking()
                           join project in projects on part.ProjectCode equals project.ProjectCode
                           orderby part.SlotName
                           select part).ToListAsync();
        return View(new QuoteHistoryViewModel
        {
            Projects = await projects.ToListAsync(),
            Parts = parts.ToLookup(part => part.ProjectCode)
        });
    }
}
