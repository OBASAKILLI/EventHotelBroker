using EventHotelBroker.Models;

namespace EventHotelBroker.Repositories;

public interface IUnitOfWork : IDisposable
{
    IHotelRepository Hotels { get; }
    IRepository<HotelImage> HotelImages { get; }
    IRepository<Service> Services { get; }
    IRepository<ServiceImage> ServiceImages { get; }
    IRepository<Category> Categories { get; }
    IRepository<Amenity> Amenities { get; }
    IRepository<HotelAmenity> HotelAmenities { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Message> Messages { get; }
    IRepository<AuditLog> AuditLogs { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
