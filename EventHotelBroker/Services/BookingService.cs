using EventHotelBroker.Models;
using EventHotelBroker.Repositories;
using EventHotelBroker.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ApplicationDbContext _context;

    public BookingService(IUnitOfWork unitOfWork, IAuditService auditService, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _context = context;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        booking.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(booking.UserId, "BookingCreated",
            $"Created booking for hotel ID {booking.HotelId} from {booking.StartDate:yyyy-MM-dd} to {booking.EndDate:yyyy-MM-dd}");

        return booking;
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByHotelAsync(int hotelId)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.User)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
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

    public async Task<bool> RejectBookingAsync(int id, string? reason = null)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Rejected;
        booking.RejectionReason = reason;
        booking.RejectedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(booking.UserId, "BookingRejected",
            $"Rejected booking ID {booking.Id}" + (!string.IsNullOrEmpty(reason) ? $" - Reason: {reason}" : ""));

        return true;
    }

    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
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
