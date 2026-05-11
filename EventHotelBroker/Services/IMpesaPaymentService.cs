using EventHotelBroker.Models;

namespace EventHotelBroker.Services
{
    public interface IMpesaPaymentService
    {
        Task<MpesaPaymentResponse> InitiatePaymentAsync(string senderPhone, string receiverPhone, decimal totalAmount, decimal commissionValue);
        Task<MpesaPaymentStatusResponse> CheckPaymentStatusAsync(string checkoutRequestId);
        Task SendBookingNotificationEmailAsync(string hotelEmail, string hotelName, string guestName, DateTime checkIn, DateTime checkOut, int guests, decimal totalAmount, string mpesaReceipt);
    }

    public class MpesaPaymentResponse
    {
        public bool Success { get; set; }
        public MpesaDeal Deal { get; set; }
        public string CheckoutRequestId { get; set; }
        public string Message { get; set; }
        public string RawResponse { get; set; }
    }

    public class MpesaDeal
    {
        public string Id { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string? MpesaCheckoutId { get; set; }
        public string? MpesaReceipt { get; set; }
        public string? B2cConversationId { get; set; }
        public string? B2cTransactionId { get; set; }
        public string? FailureReason { get; set; }
    }

    public class MpesaPaymentStatusResponse
    {
        public bool Success { get; set; }
        public MpesaDeal Deal { get; set; }
    }
}
