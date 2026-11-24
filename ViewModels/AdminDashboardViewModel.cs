namespace Nuttawat_7324_L002.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalMembers { get; set; }
    public int TotalStaff { get; set; }
    public List<AdminUserItemViewModel> Users { get; set; } = new();
}

public class AdminUserItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
}
