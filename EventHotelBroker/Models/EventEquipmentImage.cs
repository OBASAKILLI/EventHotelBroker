using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public class EventEquipmentImage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;

    public int DisplayOrder { get; set; } = 0;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("EquipmentId")]
    public virtual EventEquipment? Equipment { get; set; }
}
