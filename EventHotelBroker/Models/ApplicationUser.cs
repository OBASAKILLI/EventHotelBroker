namespace EventHotelBroker.Models;

public class ApplicationUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; } // Admin, HotelOwner, User
    public bool IsActive { get; set; } = true;
    public bool IsOwnerVerified { get; set; } = false;
    public string? BusinessName { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
}
