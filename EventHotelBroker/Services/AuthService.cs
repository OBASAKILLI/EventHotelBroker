using EventHotelBroker.Data;
using EventHotelBroker.Models;
using EventHotelBroker.Utils;
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

    public async Task<(bool Success, string Message, Users? User)> LoginAsync(string email, string password)
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.strid == userId);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("2FA: User not found for userId: {UserId}", userId);
                return false;
            }

            // Generate a 6-digit code
            var code = new Random().Next(100000, 999999).ToString();

            // Store the code and expiry time
            user.TwoFACode = code;
            user.TwoFACodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await _context.SaveChangesAsync();
            _logger.LogInformation("2FA code saved to DB for user: {UserId}, code set: {HasCode}", userId, !string.IsNullOrEmpty(user.TwoFACode));

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.strid == userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            _logger.LogInformation("2FA verify: userId={UserId}, codeInDb={Code}, expiry={Expiry}", userId, user.TwoFACode ?? "NULL", user.TwoFACodeExpiry?.ToString() ?? "NULL");

            if (string.IsNullOrEmpty(user.TwoFACode))
            {
                return (false, "No verification code found. Please request a new code.");
            }

            if (user.TwoFACodeExpiry == null || user.TwoFACodeExpiry < DateTime.UtcNow)
            {
                return (false, "Verification code has expired. Please request a new code.");
            }

            if (user.TwoFACode != code)
            {
                return (false, "Invalid verification code");
            }

            // Clear the 2FA code after successful verification
            user.TwoFACode = null;
            user.TwoFACodeExpiry = null;
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

    public async Task<Users?> GetUserByIdAsync(string userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.strid == userId);
    }

    public async Task<(bool Success, string Message)> ForgotPasswordAsync(string email, string baseUri)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // Don't reveal whether user exists
                return (true, "If an account with that email exists, a password reset link has been sent.");
            }

            // Generate reset token and set 1 hour expiry
            var resetToken = Guid.NewGuid().ToString("N");
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            // Build reset link
            var resetLink = $"{baseUri.TrimEnd('/')}/reset-password?token={resetToken}&email={Uri.EscapeDataString(email)}";
            await _emailService.SendPasswordResetEmailAsync(email, user.FullName ?? "User", resetLink);

            _logger.LogInformation("Password reset email sent to: {Email}", email);
            return (true, "If an account with that email exists, a password reset link has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for email: {Email}", email);
            return (false, "An error occurred. Please try again later.");
        }
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return (false, "Invalid reset link.");
            }

            if (string.IsNullOrEmpty(user.ResetToken) || user.ResetToken != token)
            {
                return (false, "Invalid or already used reset token.");
            }

            if (user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return (false, "Reset link has expired. Please request a new one.");
            }

            // Encrypt and set new password
            var encryption = new Encryption();
            user.password_hash = encryption.Encryptstring(newPassword);

            // Clear reset token
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset successful for email: {Email}", email);
            return (true, "Your password has been reset successfully. You can now sign in.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for email: {Email}", email);
            return (false, "An error occurred. Please try again later.");
        }
    }

    private bool VerifyPassword(string password, string? passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
        {
            return false;
        }

        var encryption = new Encryption();
        var encryptedInput = encryption.Encryptstring(password);
        return encryptedInput == passwordHash;
    }
}
