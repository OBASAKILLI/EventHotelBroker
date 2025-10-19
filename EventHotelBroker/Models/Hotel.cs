using System.ComponentModel.DataAnnotations;

namespace EventHotelBroker.Models;

public class Hotel
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Hotel name is required")]
    [StringLength(200, ErrorMessage = "Hotel name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    public string Slug { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 50, ErrorMessage = "Description must be between 50 and 2000 characters")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string Address { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters")]
    public string City { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
    public string Country { get; set; } = string.Empty;
    
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal? Latitude { get; set; }
    
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal? Longitude { get; set; }
    
    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10000")]
    public int Capacity { get; set; }
    
    [Required(ErrorMessage = "Price per night is required")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    public decimal PricePerNight { get; set; }
    
    public string Currency { get; set; } = "KES";
    public bool IsPublished { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser? Owner { get; set; }
    public virtual ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
    public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
