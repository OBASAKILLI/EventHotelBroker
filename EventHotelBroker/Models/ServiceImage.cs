namespace EventHotelBroker.Models;

public class ServiceImage
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Service? Service { get; set; }
}
