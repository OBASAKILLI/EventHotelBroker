namespace EventHotelBroker.Models;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int HotelId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int HeadCount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual Hotel? Hotel { get; set; }
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Rejected,
    Cancelled
}
