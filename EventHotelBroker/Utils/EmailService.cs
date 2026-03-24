using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace EventHotelBroker.Utils
{
    public interface IEmailService
    {
        Task<bool> SendCodeAsync(string email, string code, string displayName);
        Task<bool> SendVerificationEmailAsync(string email, string fullName, string verificationLink);
     Task<bool> SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendCodeAsync(string email, string code, string displayName)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var host = smtpSettings["Host"];
                var port = int.Parse(smtpSettings["Port"] ?? "587");
                var user = smtpSettings["User"];
                var pass = smtpSettings["Pass"];
                var from = smtpSettings["From"];
                var fromName = smtpSettings["FromName"];
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

                var codeExpiryMinutes = _configuration.GetValue<int>("TwoFactorAuth:CodeExpiryMinutes", 10);

                var subject = "Your Two-Factor Authentication Code - Standard Insurance";
                var body = Generate2FAEmailHtml(displayName, code, codeExpiryMinutes);

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(user, pass);
                    client.Timeout = 10000;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(from, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var host = smtpSettings["Host"];
                var port = int.Parse(smtpSettings["Port"] ?? "587");
                var user = smtpSettings["User"];
                var pass = smtpSettings["Pass"];
                var from = smtpSettings["From"];
                var fromName = smtpSettings["FromName"] ?? "Standard Insurance";
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

                var subject = "Reset your password - Standard Insurance";

                var safeName = string.IsNullOrWhiteSpace(fullName) ? toEmail : fullName;

                var body = $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
    <style>
        body {{ margin:0; padding:0; background:#f3f4f6; font-family:Segoe UI, Arial, sans-serif; }}
        .wrapper {{ width:100%; background:#f3f4f6; padding:24px 8px; box-sizing:border-box; }}
        .container {{ max-width:640px; margin:0 auto; background:#ffffff; border-radius:12px; box-shadow:0 18px 45px rgba(15,23,42,0.18); overflow:hidden; }}
        .header {{ background:linear-gradient(135deg,#17275F,#2A4A9F); padding:18px 24px; color:#f9fafb; }}
        .header-title {{ margin:0; font-size:18px; font-weight:700; }}
        .header-sub {{ margin:4px 0 0 0; font-size:12px; opacity:0.9; }}
        .content {{ padding:22px 24px 20px 24px; font-size:13px; color:#111827; }}
        .content p {{ margin:0 0 12px 0; line-height:1.5; }}
        .btn {{ display:inline-block; margin-top:16px; padding:10px 20px; background:#2563eb; color:#ffffff !important; text-decoration:none; border-radius:9999px; font-size:13px; font-weight:600; box-shadow:0 10px 25px rgba(37,99,235,0.35); }}
        .footer {{ margin-top:18px; font-size:11px; color:#6b7280; line-height:1.4; border-top:1px solid #e5e7eb; padding-top:12px; }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='container'>
            <div class='header'>
                <h1 class='header-title'>Password Reset Request</h1>
                <p class='header-sub'>You requested to reset your password for the Standard Insurance client portal.</p>
            </div>
            <div class='content'>
                <p>Dear {WebUtility.HtmlEncode(safeName)},</p>
                <p>We received a request to reset the password associated with this email address. If you made this request, please click the button below to choose a new password.</p>
                <p>
                    <a href='{WebUtility.HtmlEncode(resetLink)}' class='btn' target='_blank'>Reset Password</a>
                </p>
                <p>If the button does not work, copy and paste this link into your browser:</p>
                <p style='word-break:break-all; font-size:12px;'>{WebUtility.HtmlEncode(resetLink)}</p>
                <p>If you did not request a password reset, you can safely ignore this email and your password will remain unchanged.</p>
                <p>Kind regards,<br/>Standard Insurance Client Portal</p>
                <div class='footer'>
                    <div>Standard Insurance Corretores de Seguros, SA</div>
                    <div>Mozambique | Phone: +258-762-065-500</div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>";

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(user, pass);
                    client.Timeout = 10000;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(from, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Password reset email sending failed: {ex.Message}");
                return false;
            }
        }

       public async Task<bool> SendVerificationEmailAsync(string email, string fullName, string verificationLink)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");
                var host = smtpSettings["Host"];
                var port = int.Parse(smtpSettings["Port"] ?? "587");
                var user = smtpSettings["User"];
                var pass = smtpSettings["Pass"];
                var from = smtpSettings["From"];
                var fromName = smtpSettings["FromName"] ?? "Standard Insurance";
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

                var subject = "Activate Your Standard Insurance Account";
                var body = GenerateVerificationEmailHtml(fullName, verificationLink);

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(user, pass);
                    client.Timeout = 10000;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(from, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Verification email sending failed: {ex.Message}");
                return false;
            }
        }


        private string GenerateVerificationEmailHtml(string fullName, string verificationLink)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Activate Your Account</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Helvetica Neue', sans-serif;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            padding: 20px;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background: linear-gradient(135deg, #17275F 0%, #2A4A9F 100%);
            padding: 20px 16px;
            text-align: center;
            color: white;
        }}
        .email-header-logo {{
            width: 60px;
            height: 60px;
            background: rgba(255, 255, 255, 0.2);
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 20px;
            font-size: 32px;
            font-weight: bold;
        }}
        .email-header h1 {{
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 8px;
        }}
        .email-header p {{
            font-size: 14px;
            opacity: 0.9;
        }}
        .email-body {{
            padding: 16px;
        }}
        .greeting {{
            font-size: 16px;
            color: #333;
            margin-bottom: 16px;
            line-height: 1.6;
        }}
        .greeting strong {{
            color: #17275F;
        }}
        .message {{
            font-size: 14px;
            color: #666;
            line-height: 1.8;
            margin-bottom: 16px;
        }}
        .cta-button {{
            display: inline-block;
            background: linear-gradient(135deg, #FF9900 0%, #E68A00 100%);
            color: #ffffff !important;
            padding: 12px 32px;
            border-radius: 8px;
            text-decoration: none !important;
            font-weight: 700;
            font-size: 14px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(255, 153, 0, 0.3);
            display: block;
            text-align: center;
            margin: 16px 0;
            border: none;
        }}
        .cta-button:hover {{
            background: linear-gradient(135deg, #E68A00 0%, #CC7700 100%);
            box-shadow: 0 6px 20px rgba(255, 153, 0, 0.4);
            color: #ffffff !important;
        }}
        .link-text {{
            font-size: 12px;
            color: #999;
            margin-top: 16px;
            word-break: break-all;
        }}
        .link-text a {{
            color: #17275F;
            text-decoration: none;
        }}
        .features {{
            display: none;
        }}
        .email-footer {{
            background: #f8f9fa;
            padding: 16px;
            text-align: center;
            border-top: 1px solid #e5e5e7;
        }}
        .footer-text {{
            font-size: 12px;
            color: #999;
            line-height: 1.6;
        }}
        .footer-links {{
            margin-top: 16px;
        }}
        .footer-links a {{
            color: #17275F;
            text-decoration: none;
            font-size: 12px;
            margin: 0 12px;
        }}
        .support-email {{
            font-size: 12px;
            color: #666;
            margin-top: 16px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <!-- Header -->
        <div class='email-header'>
            <div class='email-header-logo'>🛡️</div>
            <h1>Welcome to Standard Insurance</h1>
            <p>Activate your account to get started</p>
        </div>

        <!-- Body -->
        <div class='email-body'>
            <div class='greeting'>
                Hi <strong>{fullName}</strong>,
            </div>

            <div class='message'>
                Thank you for creating an account with Standard Insurance! We're excited to have you on board. To complete your registration and start exploring our insurance products, please activate your account by clicking the button below.
            </div>

            <a href='{verificationLink}' class='cta-button'>Activate My Account</a>

            <div class='link-text'>
                If the button doesn't work, copy and paste this link in your browser:<br>
                <a href='{verificationLink}'>{verificationLink}</a>
            </div>

            <div class='support-email'>
                Need help? Contact us at <a href='mailto:info@standardinsurance.co.mz'>info@standardinsurance.co.mz</a>
            </div>

            <!-- Security Note -->
            <div class='security-note'>
                🔒 <strong>Security Tip:</strong> This link will expire in 24 hours. If you didn't create this account, please ignore this email.
            </div>

            <!-- Features -->
            <div class='features'>
                <h3>What You Can Do:</h3>
                <ul class='feature-list'>
                    <li>Get instant insurance quotes</li>
                    <li>Compare multiple coverage options</li>
                    <li>Manage your policies online</li>
                    <li>Access 24/7 customer support</li>
                    <li>Receive personalized recommendations</li>
                </ul>
            </div>

            <div class='message'>
                If you have any questions or need assistance, our support team is here to help!
            </div>
        </div>

        <!-- Footer -->
        <div class='email-footer'>
            <div class='footer-text'>
                <strong>Standard Insurance Corretores de Seguros, SA</strong><br>
                Mozambique | Phone: +258-21-50-1020 | Email: info@standardinsurance.co.mz<br>
                <div class='footer-links'>
                    <a href='#'>Privacy Policy</a> | 
                    <a href='#'>Terms of Service</a> | 
                    <a href='#'>Contact Us</a>
                </div>
            </div>
            <div class='footer-text' style='margin-top: 16px; font-size: 11px;'>
                &copy; 2025 Standard Insurance. All rights reserved.
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string Generate2FAEmailHtml(string displayName, string code, int expiryMinutes)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Two-Factor Authentication Code</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Helvetica Neue', sans-serif;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            padding: 20px;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
        }}
        .email-header {{
            background: linear-gradient(135deg, #17275F 0%, #2A4A9F 100%);
            padding: 20px 16px;
            text-align: center;
            color: white;
        }}
        .email-header-logo {{
            width: 60px;
            height: 60px;
            background: rgba(255, 255, 255, 0.2);
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 20px;
            font-size: 32px;
            font-weight: bold;
        }}
        .email-header h1 {{
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 8px;
        }}
        .email-header p {{
            font-size: 14px;
            opacity: 0.9;
        }}
        .email-body {{
            padding: 16px;
        }}
        .greeting {{
            font-size: 16px;
            color: #333;
            margin-bottom: 16px;
            line-height: 1.6;
        }}
        .greeting strong {{
            color: #17275F;
        }}
        .message {{
            font-size: 14px;
            color: #666;
            line-height: 1.8;
            margin-bottom: 16px;
        }}
        .code-box {{
            background: linear-gradient(135deg, #f8f9fa 0%, #f0f2f5 100%);
            border: 2px solid #17275F;
            border-radius: 12px;
            padding: 16px;
            text-align: center;
            margin: 16px 0;
        }}
        .code-label {{
            font-size: 12px;
            color: #999;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 12px;
        }}
        .code-display {{
            font-size: 32px;
            font-weight: 700;
            color: #17275F;
            letter-spacing: 6px;
            font-family: 'Courier New', monospace;
            margin: 0;
        }}
        .expiry-info {{
            font-size: 12px;
            color: #666;
            margin-top: 16px;
        }}
        .security-note {{
            background: #e8f4f8;
            border-left: 4px solid #17275F;
            padding: 16px;
            border-radius: 4px;
            margin: 24px 0;
            font-size: 12px;
            color: #333;
        }}
        .security-note strong {{
            color: #17275F;
        }}
        .warning-box {{
            background: #ffebee;
            border-left: 4px solid #d32f2f;
            padding: 16px;
            border-radius: 4px;
            margin: 24px 0;
            font-size: 12px;
            color: #333;
        }}
        .warning-box strong {{
            color: #d32f2f;
        }}
        .email-footer {{
            background: #f8f9fa;
            padding: 30px;
            text-align: center;
            border-top: 1px solid #e5e5e7;
        }}
        .footer-text {{
            font-size: 12px;
            color: #999;
            line-height: 1.6;
        }}
        .footer-links {{
            margin-top: 16px;
        }}
        .footer-links a {{
            color: #17275F;
            text-decoration: none;
            font-size: 12px;
            margin: 0 12px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <!-- Header -->
        <div class='email-header'>
            <div class='email-header-logo'>🔐</div>
            <h1>Verify Your Login</h1>
            <p>Two-Factor Authentication Code</p>
        </div>

        <!-- Body -->
        <div class='email-body'>
            <div class='greeting'>
                Hi <strong>{displayName}</strong>,
            </div>

            <div class='message'>
                You're attempting to sign in to your Standard Insurance account. To complete your login securely, please use the verification code below:
            </div>

            <!-- Code Box -->
            <div class='code-box'>
                <div class='code-label'>Your Verification Code</div>
                <div class='code-display'>{code}</div>
                <div class='expiry-info'>This code will expire in <strong>{expiryMinutes} minutes</strong></div>
            </div>

            <!-- Security Note -->
            <div class='security-note'>
                🔒 <strong>Security Tip:</strong> Never share this code with anyone. Standard Insurance staff will never ask for this code.
            </div>

            <!-- Warning -->
            <div class='warning-box'>
                ⚠️ <strong>Didn't try to log in?</strong> If you didn't attempt to sign in, please ignore this email and secure your account immediately by changing your password.
            </div>

            <div class='message'>
                If you have any questions or need assistance, our support team is here to help!
            </div>
        </div>

        <!-- Footer -->
        <div class='email-footer'>
            <div class='footer-text'>
                <strong>Standard Insurance Corretores de Seguros, SA</strong><br>
                Mozambique | Phone: +258-21-50-1020 | Email: info@standardinsurance.co.mz<br>
                <div class='footer-links'>
                    <a href='#'>Privacy Policy</a> | 
                    <a href='#'>Terms of Service</a> | 
                    <a href='#'>Contact Us</a>
                </div>
            </div>
            <div class='footer-text' style='margin-top: 16px; font-size: 11px;'>
                © 2025 Standard Insurance. All rights reserved.
            </div>
        </div>
    </div>
</body>
</html>";
        }
    }
}
