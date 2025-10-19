using EventHotelBroker.Models;
using EventHotelBroker.Repositories;

namespace EventHotelBroker.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public BookingService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        booking.CreatedAt = DateTime.UtcNow;
        booking.Status = BookingStatus.Pending;

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(booking.UserId, "BookingCreated",
            $"Created booking for hotel ID {booking.HotelId} from {booking.StartDate:yyyy-MM-dd} to {booking.EndDate:yyyy-MM-dd}");

        return booking;
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _unitOfWork.Bookings.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId)
    {
        return await _unitOfWork.Bookings.FindAsync(b => b.UserId == userId);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByHotelAsync(int hotelId)
    {
        return await _unitOfWork.Bookings.FindAsync(b => b.HotelId == hotelId);
    }

    public async Task<bool> ConfirmBookingAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(booking.UserId, "BookingConfirmed",
            $"Confirmed booking ID {booking.Id}");

        return true;
    }

    public async Task<bool> RejectBookingAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Rejected;
        booking.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(booking.UserId, "BookingRejected",
            $"Rejected booking ID {booking.Id}");

        return true;
    }

    public async Task<bool> CancelBookingAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(booking.UserId, "BookingCancelled",
            $"Cancelled booking ID {booking.Id}");

        return true;
    }
}
