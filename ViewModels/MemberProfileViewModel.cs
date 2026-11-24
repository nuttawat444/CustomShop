namespace Nuttawat_7324_L002.ViewModels;

public sealed class MemberProfileViewModel
{
    public string Code { get; set; } = string.Empty;
    public List<MemberCouponViewModel> Coupons { get; set; } = [];
    public List<MemberOffer> SavedOffers { get; set; } = [];
}

public sealed class MemberCouponViewModel
{
    public string Code { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Percent { get; set; }
    public bool Used { get; set; }
}
