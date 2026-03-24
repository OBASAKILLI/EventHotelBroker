namespace EventHotelBroker.Services;

public interface IEmailService
{
    Task SendTwoFactorCodeAsync(string email, string code, string userName);
    Task SendVerificationEmailAsync(string email, string userName, string verificationLink);
    Task SendEmailAsync(string to, string subject, string body);
}
