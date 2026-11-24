
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;

namespace Nuttawat_7324_L002.Services;

public static class OwnerDepartmentQuery
{
    public static IQueryable<OwnerDepartmentSummaryViewModel> Summaries(IQueryable<Role> roles) =>
        roles.Where(role => AppRoles.All.Contains(role.Name))
            .Select(role => new OwnerDepartmentSummaryViewModel
            {
                DepartmentName = role.Name,
                HeadCount = role.Staff.Count(staff => staff.Status == "active"),
                Status = role.Staff.Any(staff => staff.Status == "active") ? "มีพนักงานประจำ" : "ไม่มีพนักงานประจำ"
            });
}
