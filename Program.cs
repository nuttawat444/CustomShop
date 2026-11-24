using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Nuttawat_7324_L002.Models;

using Nuttawat_7324_L002.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<BikeBuildService>();
builder.Services.AddScoped<QuoteWorkService>();
builder.Services.AddScoped<QuoteEstimateService>();
builder.Services.AddScoped<WarehouseCatalogService>();
builder.Services.AddAuthorization(ShopAccess.ConfigurePolicies);
builder.Services.AddDbContext<Csi402dbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events.OnValidatePrincipal = async context =>
        {
            var principal = context.Principal;
            if (principal?.Identity is not ClaimsIdentity identity
                || !int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<Csi402dbContext>();
            var account = await dbContext.Users
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => new
                {
                    user.Name,
                    user.Email,
                    RoleName = user.Staff == null ? null : user.Staff.Role.Name
                })
                .SingleOrDefaultAsync();
            if (account is null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            var shouldRenew = false;
            var nameClaims = identity.FindAll(ClaimTypes.Name).ToList();
            if (nameClaims.Count != 1 || nameClaims[0].Value != account.Name)
            {
                foreach (var claim in nameClaims)
                {
                    identity.RemoveClaim(claim);
                }

                identity.AddClaim(new Claim(ClaimTypes.Name, account.Name));
                shouldRenew = true;
            }

            var emailClaims = identity.FindAll(ClaimTypes.Email).ToList();
            if (emailClaims.Count != 1 || emailClaims[0].Value != account.Email)
            {
                foreach (var claim in emailClaims)
                {
                    identity.RemoveClaim(claim);
                }

                identity.AddClaim(new Claim(ClaimTypes.Email, account.Email));
                shouldRenew = true;
            }

            var roleClaims = identity.FindAll(identity.RoleClaimType).ToList();
            var hasRole = !string.IsNullOrWhiteSpace(account.RoleName);
            if ((hasRole && (roleClaims.Count != 1 || roleClaims[0].Value != account.RoleName))
                || (!hasRole && roleClaims.Count != 0))
            {
                foreach (var roleClaim in roleClaims)
                {
                    identity.RemoveClaim(roleClaim);
                }

                if (hasRole)
                {
                    identity.AddClaim(new Claim(identity.RoleClaimType, account.RoleName!));
                }

                shouldRenew = true;
            }

            context.ShouldRenew = shouldRenew;
        };
    });

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Csi402dbContext>();

    // Clean up obsolete roles: Remove "Warehouse" and "Worker", keep only "Warehouse Worker"
    var rolesToRemove = await dbContext.Roles
        .Where(r => r.Name == "Warehouse" || r.Name == "Worker")
        .ToListAsync();

    if (rolesToRemove.Any())
    {
        var whWorkerRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Warehouse Worker");
        if (whWorkerRole == null)
        {
            whWorkerRole = new Role { Name = "Warehouse Worker" };
            dbContext.Roles.Add(whWorkerRole);
            await dbContext.SaveChangesAsync();
        }

        foreach (var r in rolesToRemove)
        {
            var staffWithRole = await dbContext.Staff.Where(s => s.RoleId == r.Id).ToListAsync();
            foreach (var s in staffWithRole)
            {
                s.RoleId = whWorkerRole.Id;
            }
            dbContext.Roles.Remove(r);
        }
        await dbContext.SaveChangesAsync();
    }

    foreach (var roleName in AppRoles.All)
    {
        if (!await dbContext.Roles.AnyAsync(role => role.Name == roleName))
        {
            dbContext.Roles.Add(new Role { Name = roleName });
        }
    }

    await dbContext.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
