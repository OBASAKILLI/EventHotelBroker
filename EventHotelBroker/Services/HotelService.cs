using EventHotelBroker.Models;
using EventHotelBroker.Repositories;

namespace EventHotelBroker.Services;

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public HotelService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
    {
        return await _unitOfWork.Hotels.GetAllAsync();
    }

    public async Task<IEnumerable<Hotel>> GetPublishedHotelsAsync()
    {
        return await _unitOfWork.Hotels.GetPublishedHotelsAsync();
    }

    public async Task<IEnumerable<Hotel>> GetHotelsByOwnerAsync(string ownerId)
    {
        return await _unitOfWork.Hotels.GetHotelsByOwnerAsync(ownerId);
    }

    public async Task<Hotel?> GetHotelByIdAsync(int id)
    {
        return await _unitOfWork.Hotels.GetHotelWithImagesAsync(id);
    }

    public async Task<Hotel?> GetHotelWithDetailsAsync(int id)
    {
        return await _unitOfWork.Hotels.GetHotelWithAmenitiesAsync(id);
    }

    public async Task<Hotel> CreateHotelAsync(Hotel hotel)
    {
        hotel.Slug = await GenerateUniqueSlugAsync(hotel.Name);
        hotel.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Hotels.AddAsync(hotel);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(hotel.OwnerId, "HotelCreated", 
            $"Created hotel: {hotel.Name} (ID: {hotel.Id})");

        return hotel;
    }

    public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
    {
        if (string.IsNullOrWhiteSpace(hotel.Slug))
        {
            hotel.Slug = await GenerateUniqueSlugAsync(hotel.Name);
        }
        hotel.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(hotel.OwnerId, "HotelUpdated", 
            $"Updated hotel: {hotel.Name} (ID: {hotel.Id})");

        return hotel;
    }

    public async Task DeleteHotelAsync(int id)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel != null)
        {
            _unitOfWork.Hotels.Remove(hotel);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogActionAsync(hotel.OwnerId, "HotelDeleted", 
                $"Deleted hotel: {hotel.Name} (ID: {hotel.Id})");
        }
    }

    public async Task<bool> ApproveHotelAsync(int id)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null) return false;

        hotel.IsApproved = true;
        hotel.IsPublished = true;
        hotel.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(null, "HotelApproved", 
            $"Approved hotel: {hotel.Name} (ID: {hotel.Id})");

        return true;
    }

    public async Task<bool> RejectHotelAsync(int id)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null) return false;

        hotel.IsApproved = false;
        hotel.IsPublished = false;
        hotel.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogActionAsync(null, "HotelRejected", 
            $"Rejected hotel: {hotel.Name} (ID: {hotel.Id})");

        return true;
    }

    public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string? keyword, string? city, int? minCapacity, decimal? maxPrice, string? category)
    {
        return await _unitOfWork.Hotels.SearchHotelsAsync(keyword, city, minCapacity, maxPrice, category);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name)
    {
        var cleanName = (name ?? "hotel").ToLower().Trim()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "");

        cleanName = System.Text.RegularExpressions.Regex.Replace(cleanName, @"[^a-z0-9\-]", "");
        cleanName = System.Text.RegularExpressions.Regex.Replace(cleanName, @"-+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(cleanName)) cleanName = "hotel";
        if (cleanName.Length > 180) cleanName = cleanName[..180];

        // Append a GUID string (8 hex chars) to ensure uniqueness
        var guidStr = Guid.NewGuid().ToString("N")[..8];
        var slug = $"{cleanName}-{guidStr}";

        // Guarantee uniqueness against existing database records
        while (await _unitOfWork.Hotels.AnyAsync(h => h.Slug == slug))
        {
            slug = $"{cleanName}-{Guid.NewGuid():N}";
        }

        return slug;
    }
}
