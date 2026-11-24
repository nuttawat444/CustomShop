namespace Nuttawat_7324_L002.Services;

public static class MemberCouponCampaign
{
    public static IReadOnlyList<string> Codes { get; } = Array.AsReadOnly(new[]
    {
        "SHOP03", "SHOP05", "SHOP07", "SHOP10", "HELMET08", "HELMET12",
        "PARTS08", "PARTS12", "EXHAUST08", "EXHAUST12"
    });

    public static bool CanRedeem(string code, bool previouslyUsed) =>
        !previouslyUsed || !Codes.Contains(code, StringComparer.OrdinalIgnoreCase);
}
