namespace Nuttawat_7324_L002.Models;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Owner = "Owner";
    public const string WarehouseWorker = "Warehouse Worker";
    public const string Mechanic = "Mechanic";
    public const string Account = "Account";

    public static readonly string[] All = new[]
    {
        Admin,
        Owner,
        WarehouseWorker,
        Mechanic,
        Account
    };
}
