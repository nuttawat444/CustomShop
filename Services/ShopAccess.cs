using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.Services;

public sealed record DashboardLink(string Role, string Controller, string Label);

public static class ShopAccess
{
    public const string Buyer = nameof(Buyer);

    public static bool CanBuy(ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated == true && !user.HasClaim(c => c.Type == ClaimTypes.Role);

    public const string Storefront = nameof(Storefront);
    public const string Products = nameof(Products);
    public const string CustomBikes = nameof(CustomBikes);

    private static readonly DashboardLink[] Dashboards =
    [
        new(AppRoles.Admin, "Admin", "Admin"),
        new(AppRoles.Owner, "Owner", "Owner"),
        new(AppRoles.WarehouseWorker, "WarehouseWorker", "Warehouse Worker"),
        new(AppRoles.Mechanic, "Mechanic", "Mechanic"),
        new(AppRoles.Account, "Accounting", "Accounting")
    ];

    public static DashboardLink? DashboardFor(ClaimsPrincipal user) =>
        Array.Find(Dashboards, dashboard => user.IsInRole(dashboard.Role));

    public static string LandingController(ClaimsPrincipal user) =>
        DashboardFor(user)?.Controller ?? (CanViewStorefront(user) ? "Home" : "Account");

    public static bool CanViewStorefront(ClaimsPrincipal user) =>
        !user.HasClaim(claim => claim.Type == ClaimTypes.Role) || user.IsInRole(AppRoles.Admin);

    public static bool CanViewProducts(ClaimsPrincipal user) =>
        CanViewStorefront(user) || user.IsInRole(AppRoles.WarehouseWorker);

    public static bool CanViewCustomBikes(ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated == true &&
        (CanViewStorefront(user) || user.IsInRole(AppRoles.Mechanic));

    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        options.AddPolicy(Buyer, policy => policy.RequireAuthenticatedUser().RequireAssertion(ctx => CanBuy(ctx.User)));
        options.AddPolicy(Storefront, policy => policy.RequireAssertion(ctx => CanViewStorefront(ctx.User)));
        options.AddPolicy(Products, policy => policy.RequireAssertion(ctx => CanViewProducts(ctx.User)));
        options.AddPolicy(CustomBikes, policy => policy.RequireAssertion(ctx => CanViewCustomBikes(ctx.User)));
    }
}
