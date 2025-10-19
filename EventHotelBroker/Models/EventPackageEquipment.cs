using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public class EventPackageEquipment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PackageId { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("PackageId")]
    public virtual EventPackage? Package { get; set; }

    [ForeignKey("EquipmentId")]
    public virtual EventEquipment? Equipment { get; set; }
}
