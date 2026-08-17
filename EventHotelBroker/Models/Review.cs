using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHotelBroker.Models;

public class Review
{
    public int Id { get; set; }

    // Polymorphic entity reference
    [Required]
    public string EntityType { get; set; } = "Hotel"; // e.g. "Hotel", "Restaurant", "Venue"

    [Required]
    public int EntityId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Please enter a title for your review")]
    [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please write your review")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Review must be between 10 and 2000 characters")]
    public string Comment { get; set; } = string.Empty;

    public bool IsApproved { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("UserId")]
    public virtual Users? User { get; set; }
}
