using Nuttawat_7324_L002.Models;
using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class OwnerDashboardViewModel
{
    public decimal MonthlyRevenue { get; set; }
    public decimal OpenBudget { get; set; }
    public List<MechanicOption> Mechanics { get; set; } = new();
    public int ActiveCustomProjects { get; set; }
    public int TotalStaffCount { get; set; }
    public ILookup<string, ProjectPart> ProjectParts { get; set; } = Array.Empty<ProjectPart>().ToLookup(part => part.ProjectCode);
    public List<OwnerProjectRowViewModel> Projects { get; set; } = new();
    public List<OwnerDepartmentSummaryViewModel> Departments { get; set; } = new();
    public QuoteDeliveryViewModel DeliveryForm { get; set; } = new();
    public OwnerAssignmentViewModel AssignmentForm { get; set; } = new();
    public QuoteEstimateReturnForm ReturnForm { get; set; } = new();
    public OwnerCreateProjectViewModel NewProjectForm { get; set; } = new();
}


public class OwnerDepartmentSummaryViewModel
{
    public string DepartmentName { get; set; } = string.Empty;
    public int HeadCount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class OwnerCreateProjectViewModel
{
    [Required(ErrorMessage = "กรุณากรอกชื่อโปรเจกต์")]
    [Display(Name = "ชื่อโครงการ")]
    public string ProjectName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกชื่อลูกค้า")]
    [Display(Name = "ชื่อลูกค้า")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกงบประมาณ")]
    [Range(1000, 10000000, ErrorMessage = "งบประมาณต้องมากกว่า 1,000 บาท")]
    [Display(Name = "งบประมาณโครงการ (บาท)")]
    public decimal Budget { get; set; }

    [Display(Name = "สไตล์การแต่ง")]
    public string CustomStyle { get; set; } = "Cafe Racer";
}

public sealed class OwnerProjectRowViewModel
{
    public required Project Project { get; set; }
    public QuoteEstimate? LatestSubmittedEstimate { get; set; }

    public string DisplayCode =>
        Project.ProjectCode.Length > 16 ? Project.ProjectCode[..12] + "…" : Project.ProjectCode;
}

public sealed class OwnerAssignmentViewModel : IValidatableObject
{
    [Required, StringLength(50)]
    public string ProjectCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int? MechanicUserId { get; set; }
    public string? Mechanic { get; set; }
    [Range(1, int.MaxValue)]
    public int? EstimateRevision { get; set; }

    [Range(typeof(decimal), "0", "10000000")]
    public decimal Budget { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!MechanicUserId.HasValue && string.IsNullOrWhiteSpace(Mechanic))
            yield return new ValidationResult("กรุณาเลือกช่างผู้รับผิดชอบ", [nameof(MechanicUserId)]);
    }
}
