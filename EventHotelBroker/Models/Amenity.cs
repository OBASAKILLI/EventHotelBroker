namespace EventHotelBroker.Models;

public class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
}
