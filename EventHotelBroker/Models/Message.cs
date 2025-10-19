namespace EventHotelBroker.Models;

public class Message
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public int? HotelId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser? Sender { get; set; }
    public virtual ApplicationUser? Receiver { get; set; }
    public virtual Hotel? Hotel { get; set; }
}
