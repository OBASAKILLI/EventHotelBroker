using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models
{
    public class HotelCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Rating { get; set; } = string.Empty; // e.g., "5-Star", "3-Star"

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ServiceFee { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
