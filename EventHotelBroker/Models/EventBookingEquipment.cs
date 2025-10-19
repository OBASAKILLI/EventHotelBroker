using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public class EventBookingEquipment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(12, 2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(12, 2)")]
    public decimal TotalPrice { get; set; }

    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("BookingId")]
    public virtual EventBooking? Booking { get; set; }

    [ForeignKey("EquipmentId")]
    public virtual EventEquipment? Equipment { get; set; }
}
