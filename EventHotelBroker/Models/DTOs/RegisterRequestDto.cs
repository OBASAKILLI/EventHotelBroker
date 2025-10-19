using System.ComponentModel.DataAnnotations;

namespace EventHotelBroker.Models.DTOs;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = "User"; // User, HotelOwner

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    // For HotelOwner role
    public string? BusinessName { get; set; }
    public string? RegistrationNumber { get; set; }
}
