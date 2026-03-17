namespace EventHotelBroker.Services;

public interface IEmailService
{
    Task SendTwoFactorCodeAsync(string email, string code, string userName);
    Task SendEmailAsync(string to, string subject, string body);
}
