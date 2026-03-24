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
    public virtual Users? Sender { get; set; }
    public virtual Users? Receiver { get; set; }
    public virtual Hotel? Hotel { get; set; }
}
