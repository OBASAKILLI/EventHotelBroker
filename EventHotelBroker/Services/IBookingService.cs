using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId);
    Task<IEnumerable<Booking>> GetBookingsByHotelAsync(int hotelId);
    Task<bool> ConfirmBookingAsync(int id);
    Task<bool> RejectBookingAsync(int id, string? reason = null);
    Task<bool> CancelBookingAsync(int id);
    Task<List<Booking>> GetAllBookingsAsync();
}
