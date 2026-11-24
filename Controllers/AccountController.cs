using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Controllers;

public class AccountController : Controller
{
    private readonly Csi402dbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(
        Csi402dbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var profile = new MemberProfileViewModel();
        if (!ShopAccess.CanBuy(User)) return View(profile);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var memberCodes = MemberCouponCampaign.Codes.ToArray();
        var wallet = _dbContext.SavedOffers.AsNoTracking().Where(s => s.UserId == userId);
        profile.Coupons = await _dbContext.Coupons.AsNoTracking()
            .Where(coupon => coupon.Active && memberCodes.Contains(coupon.Code))
            .OrderByDescending(coupon => coupon.Category == "*").ThenBy(coupon => coupon.Category)
            .ThenByDescending(coupon => coupon.Percent).ThenBy(coupon => coupon.Code)
            .Select(coupon => new MemberCouponViewModel
            {
                Code = coupon.Code,
                Category = coupon.Category,
                Percent = coupon.Percent,
                Used = _dbContext.Orders.Any(order => order.UserId == userId &&
                    order.CouponCode == coupon.Code && order.Discount > 0)
            }).ToListAsync();
        profile.SavedOffers = await (from saved in wallet join coupon in _dbContext.Coupons on saved.CouponId equals coupon.Id
            where coupon.Active && !memberCodes.Contains(coupon.Code)
            select new MemberOffer(coupon.Code, coupon.Category, null)).ToListAsync();
        profile.SavedOffers.AddRange(await (from saved in wallet join code in _dbContext.PackageCodes on saved.PackageCode equals code.Code
            join item in _dbContext.Catalog on code.CatalogItemId equals item.Id where item.Active
            select new MemberOffer(code.Code, item.Name, item.Id)).ToListAsync());
        return View(profile);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = ShopAccess.Buyer)]
    public async Task<IActionResult> SaveOffer(string code)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        code = code?.Trim() ?? "";
        var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Active && c.Code == code);
        var package = await (from c in _dbContext.PackageCodes join p in _dbContext.Catalog on c.CatalogItemId equals p.Id
            where p.Active && p.Kind == "Bike" && c.Code == code select c).FirstOrDefaultAsync();
        if (coupon == null && package == null)
        {
            TempData["OfferMessage"] = "ไม่พบโค้ดที่เปิดใช้งาน";
            return RedirectToAction(nameof(Index));
        }
        _dbContext.SavedOffers.Add(new SavedOffer { UserId = userId, CouponId = coupon?.Id, PackageCode = coupon == null ? package!.Code : null });
        try { await _dbContext.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.GetBaseException() is MySqlException { Number: 1062 }) { }
        TempData["OfferMessage"] = "บันทึกโค้ดในบัญชีแล้ว";
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", ShopAccess.LandingController(User));
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.EmailUser.Trim().ToLowerInvariant();
        var user = await _dbContext.Users
            .Include(candidate => candidate.Staff)
            .ThenInclude(staff => staff!.Role)
            .SingleOrDefaultAsync(candidate => candidate.Email == email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านไม่ถูกต้อง");
            return View(model);
        }

        var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านไม่ถูกต้อง");
            return View(model);
        }

        if (passwordVerification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            await _dbContext.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };
        if (user.Staff is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Staff.Role.Name));
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", ShopAccess.LandingController(principal));
    }

    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", ShopAccess.LandingController(User));
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.EmailUser.Trim().ToLowerInvariant();
        var username = model.Username.Trim().ToLowerInvariant();
        var duplicateAccount = await _dbContext.Users.AnyAsync(user =>
            user.Email == email || user.Username == username);
        if (duplicateAccount)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือชื่อผู้ใช้นี้ถูกใช้แล้ว");
            return View(model);
        }

        var user = new User
        {
            Name = model.Name.Trim(),
            Username = username,
            Email = email,
            PhoneNumber = model.Tel.Trim()
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        _dbContext.Users.Add(user);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.GetBaseException() is MySqlException { Number: 1062 })
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือชื่อผู้ใช้นี้ถูกใช้แล้ว");
            return View(model);
        }

        TempData["StatusMessage"] = "สมัครสมาชิกสำเร็จ กรุณาเข้าสู่ระบบ";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", ShopAccess.LandingController(User));
        }

        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            ModelState.AddModelError("Email", "ไม่พบผู้ใช้งานที่ใช้อีเมลนี้ในระบบ");
            return View(model);
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        await _dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = "เปลี่ยนรหัสผ่านสำเร็จแล้ว กรุณาเข้าสู่ระบบด้วยรหัสผ่านใหม่";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("/Account/Dashboard")]
    [Authorize(Roles = AppRoles.Admin)]
    public IActionResult Dashboard()
    {
        return RedirectToAction("Index", "Admin");
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public IActionResult CreateUser()
    {
        return RedirectToAction("CreateUser", "Admin");
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public IActionResult EditUser(int id)
    {
        return RedirectToAction("EditUser", "Admin", new { id });
    }
}
