using Nuttawat_7324_L002.Models;
using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class MechanicDashboardViewModel
{
    public List<MechanicOption> Mechanics { get; set; } = new();
    public int ActiveJobsCount { get; set; }
    public int CompletedTodayCount { get; set; }
    public int WaitingPartsCount { get; set; }
    public int UrgentJobsCount { get; set; }
    public List<MechanicJob> JobOrders { get; set; } = new();
    public JobProgressViewModel ProgressForm { get; set; } = new();
    public MechanicCreateJobViewModel NewJobForm { get; set; } = new();
}


public class MechanicCreateJobViewModel
{
    [Required(ErrorMessage = "กรุณากรอกรุ่นรถมอเตอร์ไซค์")]
    [Display(Name = "รุ่นมอเตอร์ไซค์")]
    public string BikeModel { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณาเลือกสไตล์คัสตอม")]
    [Display(Name = "สไตล์การแต่ง")]
    public string CustomStyle { get; set; } = "Cafe Racer";

    [Required(ErrorMessage = "กรุณาระบุรายละเอียดงาน")]
    [Display(Name = "รายละเอียดงานซ่อม / ดัดแปลง")]
    public string ServiceType { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "ช่างผู้รับผิดชอบ")]
    public int? MechanicUserId { get; set; }
    public string? AssignedMechanic { get; set; }

    [Display(Name = "ระดับความสำคัญ")]
    public string Priority { get; set; } = "Normal";
}
