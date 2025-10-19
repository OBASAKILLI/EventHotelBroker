using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public enum EventBookingStatus
{
    Pending,
    Confirmed,
    Rejected,
    Cancelled,
    Completed
}

public class EventBooking
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string UserId { get; set; } = string.Empty;

    public int? PackageId { get; set; }

    [Required]
    [StringLength(200)]
    public string EventName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    public DateTime? EventEndDate { get; set; }

    [Required]
    [StringLength(500)]
    public string Venue { get; set; } = string.Empty;

    [Required]
    public int ExpectedGuests { get; set; }

    [Required]
    [Column(TypeName = "decimal(12, 2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? DepositAmount { get; set; }

    public EventBookingStatus Status { get; set; } = EventBookingStatus.Pending;

    public string? SpecialRequests { get; set; }

    [Required]
    [StringLength(100)]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ContactPhone { get; set; } = string.Empty;

    [StringLength(200)]
    public string? ContactEmail { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    [ForeignKey("PackageId")]
    public virtual EventPackage? Package { get; set; }

    public virtual ICollection<EventBookingEquipment> BookingEquipments { get; set; } = new List<EventBookingEquipment>();
}
