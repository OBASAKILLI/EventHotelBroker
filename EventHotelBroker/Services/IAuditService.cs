using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IAuditService
{
    Task LogActionAsync(string? userId, string actionType, string details, string? ipAddress = null);
    Task<IEnumerable<AuditLog>> GetLogsAsync(int pageNumber = 1, int pageSize = 50);
    Task<IEnumerable<AuditLog>> GetLogsByUserAsync(string userId);
}
