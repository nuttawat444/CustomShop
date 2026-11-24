using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly Csi402dbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AdminController(
        Csi402dbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Staff)
            .ThenInclude(staff => staff!.Role)
            .OrderBy(user => user.Name)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TotalUsers = users.Count,
            TotalAdmins = users.Count(u => u.Staff?.Role?.Name == "Admin"),
            TotalMembers = users.Count(u => u.Staff == null || u.Staff.Role?.Name == "Member"),
            TotalStaff = users.Count(u => u.Staff != null && u.Staff.Role?.Name != "Admin" && u.Staff.Role?.Name != "Member"),
            Users = users.Select(u => new AdminUserItemViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Staff?.Role?.Name ?? "Member"
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> CreateUser()
    {
        var model = new AdminCreateUserViewModel();
        await PopulateRolesAsync(model.Roles);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(AdminCreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateRolesAsync(model.Roles);
            return View(model);
        }

        var email = model.EmailUser.Trim().ToLowerInvariant();
        var username = model.Username.Trim().ToLowerInvariant();

        var duplicateAccount = await _dbContext.Users.AnyAsync(user =>
            user.Email == email || user.Username == username);
        if (duplicateAccount)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือชื่อผู้ใช้นี้ถูกใช้งานในระบบแล้ว");
            await PopulateRolesAsync(model.Roles);
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

        if (model.RoleId.HasValue)
        {
            var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == model.RoleId.Value);
            if (roleExists)
            {
                user.Staff = new Staff
                {
                    RoleId = model.RoleId.Value,
                    Status = "active"
                };
            }
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.GetBaseException() is MySqlException { Number: 1062 })
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือชื่อผู้ใช้นี้ถูกใช้งานในระบบแล้ว");
            await PopulateRolesAsync(model.Roles);
            return View(model);
        }

        TempData["StatusMessage"] = "เพิ่มผู้ใช้งานใหม่เรียบร้อยแล้ว";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _dbContext.Users
            .Include(u => u.Staff)
            .SingleOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        var model = new AdminEditUserViewModel
        {
            Id = user.Id,
            RoleId = user.Staff?.RoleId,
            Name = user.Name,
            Username = user.Username,
            EmailUser = user.Email,
            Tel = user.PhoneNumber ?? string.Empty
        };

        await PopulateRolesAsync(model.Roles);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(int id, AdminEditUserViewModel model)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        var user = await _dbContext.Users
            .Include(u => u.Staff)
            .SingleOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        Role? selectedRole = null;
        if (model.RoleId.HasValue)
        {
            selectedRole = await _dbContext.Roles
                .SingleOrDefaultAsync(role => role.Id == model.RoleId.Value);
            if (selectedRole is null)
            {
                ModelState.AddModelError(nameof(model.RoleId), "Role ที่เลือกไม่ถูกต้อง");
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateRolesAsync(model.Roles);
            return View(model);
        }

        var email = model.EmailUser.Trim().ToLowerInvariant();
        var username = model.Username.Trim().ToLowerInvariant();

        var duplicateAccount = await _dbContext.Users.AnyAsync(candidate =>
            candidate.Id != id && (candidate.Email == email || candidate.Username == username));
        if (duplicateAccount)
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือชื่อผู้ใช้นี้ถูกใช้งานในระบบแล้ว");
            await PopulateRolesAsync(model.Roles);
            return View(model);
        }

        user.Name = model.Name.Trim();
        user.Username = username;
        user.Email = email;
        user.PhoneNumber = model.Tel.Trim();

        if (selectedRole is null)
        {
            if (user.Staff is not null)
            {
                _dbContext.Staff.Remove(user.Staff);
            }
        }
        else if (user.Staff is null)
        {
            user.Staff = new Staff
            {
                UserId = user.Id,
                RoleId = selectedRole.Id,
                Status = "active"
            };
        }
        else
        {
            user.Staff.RoleId = selectedRole.Id;
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.GetBaseException() is MySqlException { Number: 1062 })
        {
            ModelState.AddModelError(string.Empty, "อีเมลหรือชื่อผู้ใช้นี้ถูกใช้งานในระบบแล้ว");
            await PopulateRolesAsync(model.Roles);
            return View(model);
        }

        TempData["StatusMessage"] = "บันทึกข้อมูลผู้ใช้งานเรียบร้อยแล้ว";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId)
            && currentUserId == id)
        {
            TempData["ErrorMessage"] = "ไม่สามารถลบบัญชีผู้ดูแลที่กำลังใช้งานอยู่ได้";
            return RedirectToAction(nameof(Index));
        }

        var user = await _dbContext.Users
            .Include(u => u.Staff)
            .SingleOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        if (user.Staff is not null)
        {
            _dbContext.Staff.Remove(user.Staff);
        }

        if (await _dbContext.Orders.AnyAsync(o => o.UserId == id) ||
            await _dbContext.Projects.AnyAsync(p => p.UserId == id) ||
            await _dbContext.Payments.AnyAsync(p => p.UserId == id))
        {
            TempData["ErrorMessage"] = "บัญชีนี้มีประวัติธุรกรรมหรือคำขอประเมินราคา";
            return RedirectToAction(nameof(Index));
        }
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        TempData["StatusMessage"] = "ลบผู้ใช้งานออกจากระบบเรียบร้อยแล้ว";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateRolesAsync(List<AdminRoleOptionViewModel> targetList)
    {
        targetList.Clear();
        var roles = await _dbContext.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new AdminRoleOptionViewModel
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();

        targetList.AddRange(roles);
    }
}
