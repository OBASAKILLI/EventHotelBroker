using EventHotelBroker.Models;

namespace EventHotelBroker.Repositories
{
    public interface IHotelCategoryRepository
    {
        Task<IEnumerable<HotelCategory>> GetAllAsync();
        Task<HotelCategory?> GetByIdAsync(int id);
        Task<HotelCategory?> GetByNameAsync(string name);
        Task AddAsync(HotelCategory category);
        Task UpdateAsync(HotelCategory category);
        Task DeleteAsync(HotelCategory category);
    }
}
