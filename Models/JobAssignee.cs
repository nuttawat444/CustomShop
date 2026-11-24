using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nuttawat_7324_L002.Models;

[Table("shop_job_assignees")]
public sealed class JobAssignee
{
    [Key, MaxLength(50)]
    public string JobTicketNo { get; set; } = string.Empty;
    public int MechanicUserId { get; set; }
}
