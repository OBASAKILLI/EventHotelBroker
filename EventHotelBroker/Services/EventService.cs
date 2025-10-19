using EventHotelBroker.Models;
using EventHotelBroker.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Equipment Management
    public async Task<IEnumerable<EventEquipment>> GetAllEquipmentsAsync()
    {
        return await _unitOfWork.EventEquipments.GetAllAsync();
    }

    public async Task<IEnumerable<EventEquipment>> GetEquipmentsByProviderAsync(string providerId)
    {
        return await _unitOfWork.EventEquipments.FindAsync(e => e.ProviderId == providerId);
    }

    public async Task<IEnumerable<EventEquipment>> GetEquipmentsByCategoryAsync(string category)
    {
        return await _unitOfWork.EventEquipments.FindAsync(e => e.Category == category && e.IsApproved && e.IsAvailable);
    }

    public async Task<EventEquipment?> GetEquipmentByIdAsync(int id)
    {
        return await _unitOfWork.EventEquipments.GetByIdAsync(id);
    }

    public async Task<EventEquipment> CreateEquipmentAsync(EventEquipment equipment)
    {
        equipment.CreatedAt = DateTime.UtcNow;
        equipment.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.EventEquipments.AddAsync(equipment);
        await _unitOfWork.SaveChangesAsync();
        return equipment;
    }

    public async Task UpdateEquipmentAsync(EventEquipment equipment)
    {
        equipment.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventEquipments.Update(equipment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteEquipmentAsync(int id)
    {
        var equipment = await _unitOfWork.EventEquipments.GetByIdAsync(id);
        if (equipment != null)
        {
            _unitOfWork.EventEquipments.Remove(equipment);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ApproveEquipmentAsync(int id)
    {
        var equipment = await _unitOfWork.EventEquipments.GetByIdAsync(id);
        if (equipment == null) return false;

        equipment.IsApproved = true;
        equipment.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventEquipments.Update(equipment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectEquipmentAsync(int id)
    {
        var equipment = await _unitOfWork.EventEquipments.GetByIdAsync(id);
        if (equipment == null) return false;

        equipment.IsApproved = false;
        equipment.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventEquipments.Update(equipment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Package Management
    public async Task<IEnumerable<EventPackage>> GetAllPackagesAsync()
    {
        return await _unitOfWork.EventPackages.GetAllAsync();
    }

    public async Task<IEnumerable<EventPackage>> GetApprovedPackagesAsync()
    {
        return await _unitOfWork.EventPackages.FindAsync(p => p.IsApproved && p.IsActive);
    }

    public async Task<IEnumerable<EventPackage>> GetPackagesByProviderAsync(string providerId)
    {
        return await _unitOfWork.EventPackages.FindAsync(p => p.ProviderId == providerId);
    }

    public async Task<IEnumerable<EventPackage>> GetPackagesByTypeAsync(string packageType)
    {
        return await _unitOfWork.EventPackages.FindAsync(p => p.PackageType == packageType && p.IsApproved && p.IsActive);
    }

    public async Task<IEnumerable<EventPackage>> GetFeaturedPackagesAsync()
    {
        return await _unitOfWork.EventPackages.FindAsync(p => p.IsFeatured && p.IsApproved && p.IsActive);
    }

    public async Task<EventPackage?> GetPackageByIdAsync(int id)
    {
        return await _unitOfWork.EventPackages.GetByIdAsync(id);
    }

    public async Task<EventPackage> CreatePackageAsync(EventPackage package)
    {
        package.CreatedAt = DateTime.UtcNow;
        package.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.EventPackages.AddAsync(package);
        await _unitOfWork.SaveChangesAsync();
        return package;
    }

    public async Task UpdatePackageAsync(EventPackage package)
    {
        package.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventPackages.Update(package);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeletePackageAsync(int id)
    {
        var package = await _unitOfWork.EventPackages.GetByIdAsync(id);
        if (package != null)
        {
            _unitOfWork.EventPackages.Remove(package);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ApprovePackageAsync(int id)
    {
        var package = await _unitOfWork.EventPackages.GetByIdAsync(id);
        if (package == null) return false;

        package.IsApproved = true;
        package.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventPackages.Update(package);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectPackageAsync(int id)
    {
        var package = await _unitOfWork.EventPackages.GetByIdAsync(id);
        if (package == null) return false;

        package.IsApproved = false;
        package.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventPackages.Update(package);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Booking Management
    public async Task<IEnumerable<EventBooking>> GetAllBookingsAsync()
    {
        return await _unitOfWork.EventBookings.GetAllAsync();
    }

    public async Task<IEnumerable<EventBooking>> GetBookingsByUserAsync(string userId)
    {
        return await _unitOfWork.EventBookings.FindAsync(b => b.UserId == userId);
    }

    public async Task<IEnumerable<EventBooking>> GetBookingsByStatusAsync(EventBookingStatus status)
    {
        return await _unitOfWork.EventBookings.FindAsync(b => b.Status == status);
    }

    public async Task<EventBooking?> GetBookingByIdAsync(int id)
    {
        return await _unitOfWork.EventBookings.GetByIdAsync(id);
    }

    public async Task<EventBooking> CreateBookingAsync(EventBooking booking)
    {
        booking.CreatedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.Status = EventBookingStatus.Pending;
        await _unitOfWork.EventBookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateBookingAsync(EventBooking booking)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ConfirmBookingAsync(int id)
    {
        var booking = await _unitOfWork.EventBookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = EventBookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectBookingAsync(int id, string reason)
    {
        var booking = await _unitOfWork.EventBookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = EventBookingStatus.Rejected;
        booking.RejectionReason = reason;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelBookingAsync(int id)
    {
        var booking = await _unitOfWork.EventBookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = EventBookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteBookingAsync(int id)
    {
        var booking = await _unitOfWork.EventBookings.GetByIdAsync(id);
        if (booking == null) return false;

        booking.Status = EventBookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.EventBookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Statistics
    public async Task<int> GetTotalEquipmentsCountAsync()
    {
        return await _unitOfWork.EventEquipments.CountAsync();
    }

    public async Task<int> GetTotalPackagesCountAsync()
    {
        return await _unitOfWork.EventPackages.CountAsync();
    }

    public async Task<int> GetTotalBookingsCountAsync()
    {
        return await _unitOfWork.EventBookings.CountAsync();
    }

    public async Task<int> GetPendingBookingsCountAsync()
    {
        return await _unitOfWork.EventBookings.CountAsync(b => b.Status == EventBookingStatus.Pending);
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        var completedBookings = await _unitOfWork.EventBookings.FindAsync(b => b.Status == EventBookingStatus.Completed);
        return completedBookings.Sum(b => b.TotalAmount);
    }
}
