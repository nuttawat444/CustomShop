namespace Nuttawat_7324_L002.Services;

public static class ShopCalendar
{
    private static readonly TimeSpan Bangkok = TimeSpan.FromHours(7);
    public static DateTime TodayUtc => DateTime.UtcNow.Add(Bangkok).Date.Subtract(Bangkok);
    public static DateTime MonthStartUtc
    {
        get
        {
            var local = DateTime.UtcNow.Add(Bangkok);
            return new DateTime(local.Year, local.Month, 1, 0, 0, 0, DateTimeKind.Utc).Subtract(Bangkok);
        }
    }
}
