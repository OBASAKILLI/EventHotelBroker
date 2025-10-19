using EventHotelBroker.Data;
using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Repositories;

public class HotelRepository : Repository<Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Hotel>> GetPublishedHotelsAsync()
    {
        return await _dbSet
            .Include(h => h.Images)
            .Include(h => h.Owner)
            .Where(h => h.IsPublished && h.IsApproved)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Hotel>> GetHotelsByOwnerAsync(string ownerId)
    {
        return await _dbSet
            .Include(h => h.Images)
            .Where(h => h.OwnerId == ownerId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<Hotel?> GetHotelWithImagesAsync(int id)
    {
        return await _dbSet
            .Include(h => h.Images)
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hotel?> GetHotelWithAmenitiesAsync(int id)
    {
        return await _dbSet
            .Include(h => h.Images)
            .Include(h => h.HotelAmenities)
                .ThenInclude(ha => ha.Amenity)
            .Include(h => h.Owner)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string? keyword, string? city, int? minCapacity, decimal? maxPrice)
    {
        var query = _dbSet
            .Include(h => h.Images)
            .Include(h => h.Owner)
            .Where(h => h.IsPublished && h.IsApproved);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(h => h.Name.Contains(keyword) || 
                                    (h.Description != null && h.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(h => h.City.Contains(city));
        }

        if (minCapacity.HasValue)
        {
            query = query.Where(h => h.Capacity >= minCapacity.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(h => h.PricePerNight <= maxPrice.Value);
        }

        return await query.OrderByDescending(h => h.CreatedAt).ToListAsync();
    }
}
