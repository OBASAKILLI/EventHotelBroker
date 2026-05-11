using EventHotelBroker.Data;
using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Repositories
{
    public class HotelCategoryRepository : IHotelCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public HotelCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelCategory>> GetAllAsync()
        {
            return await _context.HotelCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.ServiceFee)
                .ToListAsync();
        }

        public async Task<HotelCategory?> GetByIdAsync(int id)
        {
            return await _context.HotelCategories.FindAsync(id);
        }

        public async Task<HotelCategory?> GetByNameAsync(string name)
        {
            return await _context.HotelCategories
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task AddAsync(HotelCategory category)
        {
            await _context.HotelCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HotelCategory category)
        {
            _context.HotelCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(HotelCategory category)
        {
            _context.HotelCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
