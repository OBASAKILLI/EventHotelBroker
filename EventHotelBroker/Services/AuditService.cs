using EventHotelBroker.Models;
using EventHotelBroker.Repositories;

namespace EventHotelBroker.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogActionAsync(string? userId, string actionType, string details, string? ipAddress = null)
    {
        var log = new AuditLog
        {
            UserId = userId,
            ActionType = actionType,
            Details = details,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AuditLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync(int pageNumber = 1, int pageSize = 50)
    {
        var logs = await _unitOfWork.AuditLogs.GetAllAsync();
        return logs.OrderByDescending(l => l.CreatedAt)
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize);
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByUserAsync(string userId)
    {
        return await _unitOfWork.AuditLogs.FindAsync(l => l.UserId == userId);
    }
}
