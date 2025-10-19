using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IEventService
{
    // Equipment Management
    Task<IEnumerable<EventEquipment>> GetAllEquipmentsAsync();
    Task<IEnumerable<EventEquipment>> GetEquipmentsByProviderAsync(string providerId);
    Task<IEnumerable<EventEquipment>> GetEquipmentsByCategoryAsync(string category);
    Task<EventEquipment?> GetEquipmentByIdAsync(int id);
    Task<EventEquipment> CreateEquipmentAsync(EventEquipment equipment);
    Task UpdateEquipmentAsync(EventEquipment equipment);
    Task DeleteEquipmentAsync(int id);
    Task<bool> ApproveEquipmentAsync(int id);
    Task<bool> RejectEquipmentAsync(int id);

    // Package Management
    Task<IEnumerable<EventPackage>> GetAllPackagesAsync();
    Task<IEnumerable<EventPackage>> GetApprovedPackagesAsync();
    Task<IEnumerable<EventPackage>> GetPackagesByProviderAsync(string providerId);
    Task<IEnumerable<EventPackage>> GetPackagesByTypeAsync(string packageType);
    Task<IEnumerable<EventPackage>> GetFeaturedPackagesAsync();
    Task<EventPackage?> GetPackageByIdAsync(int id);
    Task<EventPackage> CreatePackageAsync(EventPackage package);
    Task UpdatePackageAsync(EventPackage package);
    Task DeletePackageAsync(int id);
    Task<bool> ApprovePackageAsync(int id);
    Task<bool> RejectPackageAsync(int id);

    // Booking Management
    Task<IEnumerable<EventBooking>> GetAllBookingsAsync();
    Task<IEnumerable<EventBooking>> GetBookingsByUserAsync(string userId);
    Task<IEnumerable<EventBooking>> GetBookingsByStatusAsync(EventBookingStatus status);
    Task<EventBooking?> GetBookingByIdAsync(int id);
    Task<EventBooking> CreateBookingAsync(EventBooking booking);
    Task UpdateBookingAsync(EventBooking booking);
    Task<bool> ConfirmBookingAsync(int id);
    Task<bool> RejectBookingAsync(int id, string reason);
    Task<bool> CancelBookingAsync(int id);
    Task<bool> CompleteBookingAsync(int id);

    // Statistics
    Task<int> GetTotalEquipmentsCountAsync();
    Task<int> GetTotalPackagesCountAsync();
    Task<int> GetTotalBookingsCountAsync();
    Task<int> GetPendingBookingsCountAsync();
    Task<decimal> GetTotalRevenueAsync();
}
