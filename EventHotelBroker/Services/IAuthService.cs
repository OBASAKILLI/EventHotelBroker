using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, Users? User)> LoginAsync(string email, string password);
    Task<bool> GenerateAndSendTwoFactorCodeAsync(string userId);
    Task<(bool Success, string Message)> VerifyTwoFactorCodeAsync(string userId, string code);
    Task<Users?> GetUserByIdAsync(string userId);
    Task<(bool Success, string Message)> ForgotPasswordAsync(string email, string baseUri);
    Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword);
    Task<(bool Success, string Message, string? NewToken)> EnrollAsOwnerAsync(string userId);
    Task<(bool Success, string Message)> CreateAdminAsync(string fullName, string email, string phone, string password, string createdByAdminId);
}
