using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public class EventPackage
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
    public string PackageType { get; set; } = string.Empty; // Wedding, Corporate, Birthday, Conference, etc.

    [Required]
    [Column(TypeName = "decimal(12, 2)")]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? DiscountedPrice { get; set; }

    public int MinGuests { get; set; } = 0;

    public int MaxGuests { get; set; } = 0;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public string? Features { get; set; } // JSON array of features

    [Required]
    [StringLength(255)]
    public string ProviderId { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsApproved { get; set; } = false;

    public bool IsFeatured { get; set; } = false;

    public bool IsCustomizable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ProviderId")]
    public virtual ApplicationUser? Provider { get; set; }

    public virtual ICollection<EventPackageEquipment> PackageEquipments { get; set; } = new List<EventPackageEquipment>();
    public virtual ICollection<EventBooking> EventBookings { get; set; } = new List<EventBooking>();
}
