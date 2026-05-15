using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace EventHotelBroker.Services
{
    public class MpesaPaymentService : IMpesaPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MpesaPaymentService> _logger;
        private readonly IEmailService _emailService;

        public MpesaPaymentService(HttpClient httpClient, IConfiguration configuration, ILogger<MpesaPaymentService> logger, IEmailService emailService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<MpesaPaymentResponse> InitiatePaymentAsync(string senderPhone, string receiverPhone, decimal totalAmount, decimal commissionValue)
        {
            try
            {
                var apiUrl = _configuration["ClemPay:ApiUrl"] ?? "https://clempay.com/api/broker/deals";
                var apiKey = _configuration["ClemPay:ApiKey"] ?? "cp_live_8bf0821a36f6d5d49291aa68e5f974d4640df0ed826e6436d9c4d69cfd649bc9";
                
                var payload = new
                {
                    senderPhone = senderPhone,
                    receiverPhone = receiverPhone,
                    receiverType = "PHONE",
                    totalAmount = totalAmount,
                    commissionType = "FIXED",
                    commissionValue = commissionValue,
                    initiate = true
                };

                _logger.LogInformation($"Initiating M-Pesa payment: {JsonSerializer.Serialize(payload)}");

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("x-api-key", apiKey);
                request.Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"M-Pesa payment initiation failed: {response.StatusCode} - {errorContent}");
                    return new MpesaPaymentResponse
                    {
                        Success = false,
                        Message = $"Payment initiation failed: {response.ReasonPhrase}"
                    };
                }

                var rawJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MpesaPaymentResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.RawResponse = rawJson;
                _logger.LogInformation($"M-Pesa payment initiated successfully: {result.CheckoutRequestId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating M-Pesa payment");
                return new MpesaPaymentResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<MpesaPaymentStatusResponse> CheckPaymentStatusAsync(string checkoutRequestId)
        {
            try
            {
                var apiUrl = $"{_configuration["ClemPay:ApiUrl"]}/status?checkoutRequestId={checkoutRequestId}";
                var apiKey = _configuration["ClemPay:ApiKey"] ?? "cp_live_8bf0821a36f6d5d49291aa68e5f974d4640df0ed826e6436d9c4d69cfd649bc9";
                
                _logger.LogInformation($"Checking M-Pesa payment status at URL: {apiUrl}");

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("x-api-key", apiKey);

                var response = await _httpClient.SendAsync(request);
                var rawJson = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Status check response ({response.StatusCode}): {rawJson}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"M-Pesa status check failed: {response.StatusCode} - {rawJson}");
                    return new MpesaPaymentStatusResponse
                    {
                        Success = false,
                        Deal = new MpesaDeal { Status = "ERROR" },
                        RawResponse = $"HTTP {response.StatusCode}: {rawJson}"
                    };
                }

                var result = JsonSerializer.Deserialize<MpesaPaymentStatusResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result != null)
                {
                    result.RawResponse = rawJson;
                }
                else
                {
                    result = new MpesaPaymentStatusResponse
                    {
                        Success = false,
                        Deal = new MpesaDeal { Status = "ERROR" },
                        RawResponse = rawJson
                    };
                }
                _logger.LogInformation($"M-Pesa payment status: {result.Deal?.Status ?? "UNKNOWN"}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking M-Pesa payment status");
                return new MpesaPaymentStatusResponse
                {
                    Success = false,
                    Deal = new MpesaDeal { Status = "ERROR" },
                    RawResponse = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task SendBookingNotificationEmailAsync(string hotelEmail, string hotelName, string guestName, DateTime checkIn, DateTime checkOut, int guests, decimal totalAmount, string mpesaReceipt, int bookingId, string bookingReference)
        {
            try
            {
                _logger.LogInformation($"Sending booking notification to {hotelEmail}");
                
                var subject = $"New Booking Received: {hotelName} - M-Pesa Confirmed";
                var bookingsLink = "https://localhost:7180/owner/bookings";
                
                var emailBody = $@"
        <html>
        <body style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f0f2f5; margin: 0; padding: 0;'>
            <div style='max-width: 600px; margin: 40px auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.08);'>
                <div style='background: linear-gradient(135deg, #2A2A2A 0%, #4a4a4a 100%); padding: 36px 32px; text-align: center;'>
                    <div style='font-size: 28px; font-weight: 700; color: #9E8B63; letter-spacing: 1px; margin-bottom: 6px;'>Safari Vents</div>
                    <div style='font-size: 14px; color: rgba(255,255,255,0.7);'>New Booking Received</div>
                </div>
                <div style='padding: 36px 32px;'>
                    <p style='font-size: 16px; color: #333; margin: 0 0 8px;'>Dear Hotel Owner,</p>
                    <p style='font-size: 14px; color: #555; line-height: 1.6; margin: 0 0 24px;'>
                        Great news! A new booking has been made at your hotel: <strong>{hotelName}</strong>. The payment was successful via M-Pesa.
                    </p>
                    
                    <div style='background: #f8f9fa; padding: 24px; border-radius: 12px; margin-bottom: 28px; border-left: 4px solid #C87941;'>
                        <h3 style='font-size: 16px; color: #9E8B63; margin: 0 0 12px; text-transform: uppercase; letter-spacing: 1px;'>Booking Details</h3>
                        <ul style='margin: 0; padding-left: 0; list-style: none; color: #4a4a4a; font-size: 14px; line-height: 2.0;'>
                            <li><strong>Booking ID:</strong> #{bookingId}</li>
                            <li><strong>Booking Reference:</strong> {bookingReference}</li>
                            <li><strong>Guest Name:</strong> {guestName}</li>
                            <li><strong>Check-in:</strong> {checkIn:MMM dd, yyyy}</li>
                            <li><strong>Check-out:</strong> {checkOut:MMM dd, yyyy}</li>
                            <li><strong>Guests:</strong> {guests}</li>
                            <li><strong>Total Amount Paid:</strong> KES {totalAmount:N0}</li>
                            <li><strong>M-Pesa Receipt:</strong> <span style='font-family: monospace; background: #e9ecef; padding: 2px 6px; border-radius: 4px;'>{mpesaReceipt}</span></li>
                        </ul>
                    </div>

                    <div style='text-align: center; margin: 0 0 28px;'>
                        <a href='{bookingsLink}' style='background: linear-gradient(135deg, #C87941 0%, #9E8B63 100%); color: #ffffff; padding: 16px 48px; border-radius: 10px; text-decoration: none; font-size: 16px; font-weight: 600; display: inline-block; letter-spacing: 0.5px;'>
                            View Booking Details
                        </a>
                    </div>
                </div>
                <div style='background: #f8f9fa; padding: 20px 32px; text-align: center; border-top: 1px solid #e9ecef;'>
                    <p style='color: #9E8B63; font-size: 13px; font-weight: 600; margin: 0 0 4px;'>Safari Vents</p>
                    <p style='color: #999; font-size: 11px; margin: 0;'>&copy; {DateTime.UtcNow.Year} Safari Vents. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

                _logger.LogInformation($"Email body prepared. Sending email via EmailService...");
                
                await _emailService.SendEmailAsync(hotelEmail, subject, emailBody);
                
                _logger.LogInformation($"Booking notification email sent successfully to {hotelEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending booking notification to {hotelEmail}");
            }
        }
    }
}
