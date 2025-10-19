namespace EventHotelBroker.Models;

public class Service
{
    public int Id { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "KES";
    public bool IsPublished { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser? Provider { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<ServiceImage> Images { get; set; } = new List<ServiceImage>();
}
