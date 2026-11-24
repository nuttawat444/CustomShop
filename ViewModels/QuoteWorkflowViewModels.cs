using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public sealed class MechanicOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Label => $"{Name} (@{Username})";
}

public sealed class JobProgressViewModel
{
    [Required, StringLength(50)]
    public string TicketNo { get; set; } = string.Empty;

    [Required]
    public bool? Complete { get; set; }
}

public sealed class QuoteDeliveryViewModel
{
    [Required, StringLength(50)]
    public string ProjectCode { get; set; } = string.Empty;
}
