namespace EventHotelBroker.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
}
