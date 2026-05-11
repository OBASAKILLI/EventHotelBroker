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

        public MpesaPaymentService(HttpClient httpClient, IConfiguration configuration, ILogger<MpesaPaymentService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
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
                
                _logger.LogInformation($"Checking M-Pesa payment status: {checkoutRequestId}");

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("x-api-key", apiKey);

                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"M-Pesa status check failed: {response.StatusCode} - {errorContent}");
                    return new MpesaPaymentStatusResponse
                    {
                        Success = false,
                        Deal = new MpesaDeal { Status = "ERROR" }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<MpesaPaymentStatusResponse>();
                _logger.LogInformation($"M-Pesa payment status: {result.Deal.Status}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking M-Pesa payment status");
                return new MpesaPaymentStatusResponse
                {
                    Success = false,
                    Deal = new MpesaDeal { Status = "ERROR" }
                };
            }
        }

        public async Task SendBookingNotificationEmailAsync(string hotelEmail, string hotelName, string guestName, DateTime checkIn, DateTime checkOut, int guests, decimal totalAmount, string mpesaReceipt)
        {
            try
            {
                _logger.LogInformation($"Sending booking notification to {hotelEmail}");
                
                // TODO: Implement email sending logic
                // You can use services like SendGrid, Mailgun, or SMTP
                
                var emailBody = $@"
                    <h2>New Booking Received</h2>
                    <p>Dear Hotel Owner,</p>
                    <p>A new booking has been made at your hotel: <strong>{hotelName}</strong></p>
                    <h3>Booking Details:</h3>
                    <ul>
                        <li><strong>Guest Name:</strong> {guestName}</li>
                        <li><strong>Check-in:</strong> {checkIn:yyyy-MM-dd}</li>
                        <li><strong>Check-out:</strong> {checkOut:yyyy-MM-dd}</li>
                        <li><strong>Number of Guests:</strong> {guests}</li>
                        <li><strong>Total Amount:</strong> KES {totalAmount:N2}</li>
                        <li><strong>M-Pesa Receipt:</strong> {mpesaReceipt}</li>
                    </ul>
                    <p>Please log in to your dashboard to view and manage this booking.</p>
                    <p>Thank you for using EventHotelBroker!</p>
                ";

                _logger.LogInformation($"Email body prepared: {emailBody}");
                
                // Simulate email sending
                await Task.Delay(100);
                
                _logger.LogInformation($"Booking notification email sent to {hotelEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending booking notification to {hotelEmail}");
            }
        }
    }
}
