using EventHotelBroker.Data;
using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, IEmailService emailService, ILogger<AuthService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, ApplicationUser? User)> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return (false, "Invalid email or password", null);
            }

            if (!user.IsActive)
            {
                return (false, "Your account has been deactivated", null);
            }

            // In a real application, you would verify the password hash here
            // For now, we'll use a simple check (you should implement proper password hashing)
            if (!VerifyPassword(password, user.PasswordHash))
            {
                return (false, "Invalid email or password", null);
            }

            return (true, "Login successful", user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", email);
            return (false, "An error occurred during login", null);
        }
    }

    public async Task<bool> GenerateAndSendTwoFactorCodeAsync(string userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return false;
            }

            // Generate a 6-digit code
            var code = new Random().Next(100000, 999999).ToString();

            // Store the code and expiry time
            user.TwoFactorCode = code;
            user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await _context.SaveChangesAsync();

            // Send the code via email
            await _emailService.SendTwoFactorCodeAsync(user.Email, code, user.FullName ?? "User");

            _logger.LogInformation("2FA code generated and sent to user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating 2FA code for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<(bool Success, string Message)> VerifyTwoFactorCodeAsync(string userId, string code)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (string.IsNullOrEmpty(user.TwoFactorCode))
            {
                return (false, "No verification code found. Please request a new code.");
            }

            if (user.TwoFactorCodeExpiry == null || user.TwoFactorCodeExpiry < DateTime.UtcNow)
            {
                return (false, "Verification code has expired. Please request a new code.");
            }

            if (user.TwoFactorCode != code)
            {
                return (false, "Invalid verification code");
            }

            // Clear the 2FA code after successful verification
            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiry = null;
            await _context.SaveChangesAsync();

            _logger.LogInformation("2FA verification successful for user: {UserId}", userId);
            return (true, "Verification successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying 2FA code for user: {UserId}", userId);
            return (false, "An error occurred during verification");
        }
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    private bool VerifyPassword(string password, string? passwordHash)
    {
        // For demo purposes, we'll use a simple comparison
        // In production, use BCrypt or ASP.NET Core Identity's password hasher
        if (string.IsNullOrEmpty(passwordHash))
        {
            // If no password hash exists, accept any password for demo
            return true;
        }

        // Simple hash comparison (replace with proper password verification)
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
