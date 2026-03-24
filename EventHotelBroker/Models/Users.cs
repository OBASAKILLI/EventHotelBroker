using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Models
{
    /// <summary>
    /// Custom validation attribute for strong password requirements
    /// </summary>
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Password is required");
            }

            string password = value.ToString();

            // Check minimum length
            if (password.Length < 8)
            {
                return new ValidationResult("Password must be at least 8 characters long");
            }

            // Check for lowercase letter
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return new ValidationResult("Password must include at least one lowercase letter");
            }

            // Check for uppercase letter
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return new ValidationResult("Password must include at least one uppercase letter");
            }

            // Check for number
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return new ValidationResult("Password must include at least one number");
            }

            // Check for symbol
            if (!Regex.IsMatch(password, "[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]"))
            {
                return new ValidationResult("Password must include at least one symbol (!@#$%^&*()_+-=[]{}';:\"\\|,.<>/?");
            }

            return ValidationResult.Success;
        }
    }

    public class Users
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string strid { get; set; }

        // Alias so code using user.Id works seamlessly
        [NotMapped]
        public string Id { get => strid; set => strid = value; }

        [Required(ErrorMessage = "Required Field")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required Field")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Required Field")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [StrongPassword]
        public string password_hash { get; set; }

        // Alias for password_hash
        [NotMapped]
        public string? PasswordHash { get => password_hash; set => password_hash = value; }

        public string FullName { get; set; }

        // 2FA Fields
        public string? TwoFACode { get; set; }
        public DateTime? TwoFACodeExpiry { get; set; }
        public bool IsTwoFAEnabled { get; set; } = true;
        public string? Contact_No { get; set; }

        // Aliases for 2FA fields used by AuthService
        [NotMapped]
        public string? TwoFactorCode { get => TwoFACode; set => TwoFACode = value; }
        [NotMapped]
        public DateTime? TwoFactorCodeExpiry { get => TwoFACodeExpiry; set => TwoFACodeExpiry = value; }
        [NotMapped]
        public bool TwoFactorEnabled { get => IsTwoFAEnabled; set => IsTwoFAEnabled = value; }

        // Role & account properties
        public string Role { get; set; } = "User";
        public string? AccountType { get; set; }
        public string? BusinessName { get; set; }
        public string? RegistrationNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOwnerVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Email Verification Fields
        public bool IsEmailVerified { get; set; } = false;
        public DateTime? EmailVerificationTimestamp { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        // Company Representative Type (for KYC Corporate flow)
        public string? CompanyRepresentative { get; set; }
        public string? CompanyName { get; set; }


        // Password Reset Fields
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // Navigation properties
        public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }

    public class EmailSendStatus
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
}
