using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, ApplicationUser? User)> LoginAsync(string email, string password);
    Task<bool> GenerateAndSendTwoFactorCodeAsync(string userId);
    Task<(bool Success, string Message)> VerifyTwoFactorCodeAsync(string userId, string code);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
}
