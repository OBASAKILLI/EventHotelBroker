using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public class EventEquipment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty; // Tents, Chairs, Sound System, Lighting, Catering, Decoration

    [Required]
    [Column(TypeName = "decimal(12, 2)")]
    public decimal PricePerUnit { get; set; }

    [Required]
    public int AvailableQuantity { get; set; }

    [StringLength(50)]
    public string Unit { get; set; } = "piece"; // piece, set, hour, day

    public string? Specifications { get; set; } // JSON or text with detailed specs

    [Required]
    [StringLength(255)]
    public string ProviderId { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ProviderId")]
    public virtual ApplicationUser? Provider { get; set; }

    public virtual ICollection<EventEquipmentImage> Images { get; set; } = new List<EventEquipmentImage>();
    public virtual ICollection<EventPackageEquipment> PackageEquipments { get; set; } = new List<EventPackageEquipment>();
    public virtual ICollection<EventBookingEquipment> BookingEquipments { get; set; } = new List<EventBookingEquipment>();
}
