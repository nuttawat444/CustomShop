using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nuttawat_7324_L002.Models;

[Table("shop_home_pins")]
public sealed class HomePin
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CatalogItemId { get; set; }
    public bool IsPinned { get; set; }
    public DateTime PinnedAt { get; set; }
}
