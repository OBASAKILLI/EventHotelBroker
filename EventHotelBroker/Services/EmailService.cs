using System.Net;
using System.Net.Mail;

namespace EventHotelBroker.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendTwoFactorCodeAsync(string email, string code, string userName)
    {
        var subject = "Your Two-Factor Authentication Code";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #667eea;'>Two-Factor Authentication</h2>
                    <p>Hello {userName},</p>
                    <p>Your verification code is:</p>
                    <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 8px; margin: 20px 0;'>
                        <h1 style='color: #667eea; font-size: 36px; letter-spacing: 8px; margin: 0;'>{code}</h1>
                    </div>
                    <p>This code will expire in 10 minutes.</p>
                    <p>If you didn't request this code, please ignore this email.</p>
                    <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
                    <p style='color: #6c757d; font-size: 12px;'>
                        This is an automated message from EventHotelBroker. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendVerificationEmailAsync(string email, string userName, string verificationLink)
    {
        var subject = "Activate Your EventHotelBroker Account";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f6f9; margin: 0; padding: 0;'>
                <div style='max-width: 600px; margin: 30px auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 32px 24px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>Welcome to EventHotelBroker</h1>
                    </div>
                    <div style='padding: 32px 24px;'>
                        <p style='font-size: 16px; color: #333;'>Hello <strong>{userName}</strong>,</p>
                        <p style='font-size: 14px; color: #555; line-height: 1.6;'>
                            Thank you for creating your account! Please click the button below to verify your email address and activate your account.
                        </p>
                        <div style='text-align: center; margin: 32px 0;'>
                            <a href='{verificationLink}' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; padding: 14px 40px; border-radius: 8px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block;'>
                                Activate My Account
                            </a>
                        </div>
                        <p style='font-size: 13px; color: #888; line-height: 1.5;'>
                            This activation link will expire in <strong>24 hours</strong>. If you did not create this account, please ignore this email.
                        </p>
                        <p style='font-size: 13px; color: #888; line-height: 1.5;'>
                            If the button above doesn't work, copy and paste the following link into your browser:
                        </p>
                        <p style='font-size: 12px; color: #667eea; word-break: break-all;'>{verificationLink}</p>
                    </div>
                    <div style='background: #f8f9fa; padding: 16px 24px; text-align: center; border-top: 1px solid #e9ecef;'>
                        <p style='color: #6c757d; font-size: 12px; margin: 0;'>
                            &copy; {DateTime.UtcNow.Year} EventHotelBroker. All rights reserved.
                        </p>
                    </div>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? _configuration["Smtp:Port"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? _configuration["Smtp:From"];
            var senderName = _configuration["EmailSettings:SenderName"] ?? _configuration["Smtp:FromName"];
            var username = _configuration["EmailSettings:SenderEmail"] ?? _configuration["Smtp:User"];
            var password = _configuration["EmailSettings:SenderPassword"] ?? _configuration["Smtp:Pass"];

            // If credentials are not configured, log the email instead
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email credentials not configured. Email would be sent to: {Email}", to);
                _logger.LogInformation("Email Subject: {Subject}", subject);
                _logger.LogInformation("Email Body: {Body}", body);
                return;
            }

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(username, password);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail ?? "noreply@eventhotelbroker.com", senderName ?? "EventHotelBroker"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }
}
