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

    private string WrapInTemplate(string headerTitle, string innerContent)
    {
        return $@"
        <html>
        <body style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f0f2f5; margin: 0; padding: 0;'>
            <div style='max-width: 600px; margin: 40px auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.08);'>
                <!-- Header -->
                <div style='background: linear-gradient(135deg, #2A2A2A 0%, #4a4a4a 100%); padding: 36px 32px; text-align: center;'>
                    <div style='font-size: 28px; font-weight: 700; color: #9E8B63; letter-spacing: 1px; margin-bottom: 6px;'>Safari Vents</div>
                    <div style='font-size: 14px; color: rgba(255,255,255,0.7);'>{headerTitle}</div>
                </div>
                <!-- Body -->
                <div style='padding: 36px 32px;'>
                    {innerContent}
                </div>
                <!-- Footer -->
                <div style='background: #f8f9fa; padding: 20px 32px; text-align: center; border-top: 1px solid #e9ecef;'>
                    <p style='color: #9E8B63; font-size: 13px; font-weight: 600; margin: 0 0 4px;'>Safari Vents</p>
                    <p style='color: #999; font-size: 11px; margin: 0;'>&copy; {DateTime.UtcNow.Year} Safari Vents. All rights reserved.</p>
                    <p style='color: #bbb; font-size: 11px; margin: 4px 0 0;'>This is an automated message. Please do not reply.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public async Task SendTwoFactorCodeAsync(string email, string code, string userName)
    {
        var subject = "Your Verification Code - Safari Vents";
        var inner = $@"
            <p style='font-size: 16px; color: #333; margin: 0 0 8px;'>Hello <strong>{userName}</strong>,</p>
            <p style='font-size: 14px; color: #555; line-height: 1.6; margin: 0 0 24px;'>
                Use the following code to complete your sign-in. This code is valid for <strong>10 minutes</strong>.
            </p>
            <div style='background: linear-gradient(135deg, #f8f6f3 0%, #f0ede8 100%); padding: 28px; text-align: center; border-radius: 12px; margin: 0 0 24px; border: 2px dashed #9E8B63;'>
                <div style='color: #2A2A2A; font-size: 42px; letter-spacing: 12px; font-weight: 700; font-family: monospace;'>{code}</div>
            </div>
            <p style='font-size: 13px; color: #888; line-height: 1.5; margin: 0;'>
                If you didn't request this code, someone may be trying to access your account. You can safely ignore this email.
            </p>";

        await SendEmailAsync(email, subject, WrapInTemplate("Two-Factor Authentication", inner));
    }

    public async Task SendVerificationEmailAsync(string email, string userName, string verificationLink)
    {
        var subject = "Activate Your Account - Safari Vents";
        var inner = $@"
            <p style='font-size: 16px; color: #333; margin: 0 0 8px;'>Hello <strong>{userName}</strong>,</p>
            <p style='font-size: 14px; color: #555; line-height: 1.6; margin: 0 0 24px;'>
                Thank you for joining Safari Vents! Click the button below to verify your email and activate your account.
            </p>
            <div style='text-align: center; margin: 0 0 28px;'>
                <a href='{verificationLink}' style='background: linear-gradient(135deg, #2A2A2A 0%, #4a4a4a 100%); color: #9E8B63; padding: 16px 48px; border-radius: 10px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block; letter-spacing: 0.5px;'>
                    Activate My Account
                </a>
            </div>
            <p style='font-size: 13px; color: #888; line-height: 1.5; margin: 0 0 12px;'>
                This link will expire in <strong>24 hours</strong>. If you did not create this account, please ignore this email.
            </p>
            <p style='font-size: 12px; color: #aaa; margin: 0 0 4px;'>If the button doesn't work, copy this link:</p>
            <p style='font-size: 12px; color: #9E8B63; word-break: break-all; margin: 0;'>{verificationLink}</p>";

        await SendEmailAsync(email, subject, WrapInTemplate("Account Activation", inner));
    }

    public async Task SendPasswordResetEmailAsync(string email, string userName, string resetLink)
    {
        var subject = "Reset Your Password - Safari Vents";
        var inner = $@"
            <p style='font-size: 16px; color: #333; margin: 0 0 8px;'>Hello <strong>{userName}</strong>,</p>
            <p style='font-size: 14px; color: #555; line-height: 1.6; margin: 0 0 24px;'>
                We received a request to reset your password. Click the button below to set a new password for your account.
            </p>
            <div style='text-align: center; margin: 0 0 28px;'>
                <a href='{resetLink}' style='background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: #ffffff; padding: 16px 48px; border-radius: 10px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block; letter-spacing: 0.5px;'>
                    Reset My Password
                </a>
            </div>
            <div style='background: #fff5f5; padding: 16px; border-radius: 8px; border-left: 4px solid #dc3545; margin: 0 0 20px;'>
                <p style='font-size: 13px; color: #dc3545; margin: 0; font-weight: 500;'>
                    ⏰ This link will expire in <strong>1 hour</strong>.
                </p>
            </div>
            <p style='font-size: 13px; color: #888; line-height: 1.5; margin: 0 0 12px;'>
                If you did not request a password reset, you can safely ignore this email. Your password will remain unchanged.
            </p>
            <p style='font-size: 12px; color: #aaa; margin: 0 0 4px;'>If the button doesn't work, copy this link:</p>
            <p style='font-size: 12px; color: #9E8B63; word-break: break-all; margin: 0;'>{resetLink}</p>";

        await SendEmailAsync(email, subject, WrapInTemplate("Password Reset", inner));
    }

    public async Task SendHotelInviteAsync(string emailAddress)
    {
        var subject = "Exclusive Invitation to Partner with Safari Vents";
        var enrollmentLink = "https://localhost:7180/owner/create-hotel";
        
        var inner = $@"
            <div style='text-align: center; margin-bottom: 24px;'>
                <p style='font-size: 20px; font-weight: 700; color: #2A2A2A; margin: 0 0 12px;'>You're Invited!</p>
                <p style='font-size: 15px; color: #555; line-height: 1.6; margin: 0;'>
                    We identified your prestigious property as a perfect candidate to join <strong>Safari Vents</strong> — the premier web platform revolutionizing how high-value clients discover and book exceptional venues.
                </p>
            </div>
            
            <div style='background: linear-gradient(135deg, #f5f3ef 0%, #ebe6dd 100%); padding: 24px; border-radius: 12px; margin-bottom: 28px; border-left: 4px solid #C87941;'>
                <h3 style='font-size: 16px; color: #9E8B63; margin: 0 0 12px; text-transform: uppercase; letter-spacing: 1px;'>Why Safari Vents?</h3>
                <ul style='margin: 0; padding-left: 20px; color: #4a4a4a; font-size: 14px; line-height: 1.7;'>
                    <li><strong>Global Audience:</strong> Showcase your hotel, event spaces, and exclusive packages to thousands of potential clients.</li>
                    <li><strong>Effortless Management:</strong> Utilize our state-of-the-art Owner Dashboard to handle booking requests and track analytics perfectly.</li>
                    <li><strong>Monetize Amenities:</strong> Lease event equipment seamlessly alongside room bookings.</li>
                </ul>
            </div>

            <p style='font-size: 15px; color: #4a4a4a; text-align: center; margin: 0 0 20px;'>
                Are you ready to elevate your business footprint?
            </p>

            <div style='text-align: center; margin: 0 0 28px;'>
                <a href='{enrollmentLink}' style='background: linear-gradient(135deg, #C87941 0%, #9E8B63 100%); color: #ffffff; padding: 18px 48px; border-radius: 12px; text-decoration: none; font-size: 16px; font-weight: 700; display: inline-block; letter-spacing: 0.5px; box-shadow: 0 8px 24px rgba(200, 121, 65, 0.3);'>
                    Enroll as a Hotel Partner
                </a>
            </div>

            <p style='font-size: 12px; color: #aaa; text-align: center; margin: 0 0 4px;'>If the button doesn't work, securely copy and paste this link:</p>
            <p style='font-size: 12px; color: #488C82; word-break: break-all; text-align: center; margin: 0;'>{enrollmentLink}</p>";

        await SendEmailAsync(emailAddress, subject, WrapInTemplate("Partner Invitation", inner));
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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Email credentials not configured. Email would be sent to: {Email}", to);
                return;
            }

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(username, password);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail ?? "noreply@safarivents.com", senderName ?? "Safari Vents"),
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
