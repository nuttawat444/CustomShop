using Nuttawat_7324_L002.Models;

namespace Nuttawat_7324_L002.ViewModels;

public class MechanicDashboardViewModel
{
    public int ActiveJobsCount { get; set; }
    public int CompletedTodayCount { get; set; }
    public int WaitingPartsCount { get; set; }
    public List<MechanicJob> JobOrders { get; set; } = new();
    public JobProgressViewModel ProgressForm { get; set; } = new();
}
