namespace EventHotelBroker.Models;

public class HotelAmenity
{
    public int HotelId { get; set; }
    public int AmenityId { get; set; }

    // Navigation properties
    public virtual Hotel? Hotel { get; set; }
    public virtual Amenity? Amenity { get; set; }
}
