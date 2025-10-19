using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IHotelService
{
    Task<IEnumerable<Hotel>> GetAllHotelsAsync();
    Task<IEnumerable<Hotel>> GetPublishedHotelsAsync();
    Task<IEnumerable<Hotel>> GetHotelsByOwnerAsync(string ownerId);
    Task<Hotel?> GetHotelByIdAsync(int id);
    Task<Hotel?> GetHotelWithDetailsAsync(int id);
    Task<Hotel> CreateHotelAsync(Hotel hotel);
    Task<Hotel> UpdateHotelAsync(Hotel hotel);
    Task DeleteHotelAsync(int id);
    Task<bool> ApproveHotelAsync(int id);
    Task<bool> RejectHotelAsync(int id);
    Task<IEnumerable<Hotel>> SearchHotelsAsync(string? keyword, string? city, int? minCapacity, decimal? maxPrice);
}
