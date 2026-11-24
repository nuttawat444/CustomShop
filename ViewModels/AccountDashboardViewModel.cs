namespace Nuttawat_7324_L002.ViewModels;

public class AccountDashboardViewModel
{
    public IReadOnlyList<AccountDashboardUserViewModel> Users { get; init; } =
        Array.Empty<AccountDashboardUserViewModel>();
}

public class AccountDashboardUserViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; }

    public string Role { get; init; } = string.Empty;
}
