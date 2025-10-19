using EventHotelBroker.Models;

namespace EventHotelBroker.Repositories;

public interface IHotelRepository : IRepository<Hotel>
{
    Task<IEnumerable<Hotel>> GetPublishedHotelsAsync();
    Task<IEnumerable<Hotel>> GetHotelsByOwnerAsync(string ownerId);
    Task<Hotel?> GetHotelWithImagesAsync(int id);
    Task<Hotel?> GetHotelWithAmenitiesAsync(int id);
    Task<IEnumerable<Hotel>> SearchHotelsAsync(string? keyword, string? city, int? minCapacity, decimal? maxPrice);
}
