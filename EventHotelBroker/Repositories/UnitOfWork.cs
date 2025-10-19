using EventHotelBroker.Data;
using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventHotelBroker.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IHotelRepository Hotels { get; }
    public IRepository<HotelImage> HotelImages { get; }
    public IRepository<Service> Services { get; }
    public IRepository<ServiceImage> ServiceImages { get; }
    public IRepository<Category> Categories { get; }
    public IRepository<Amenity> Amenities { get; }
    public IRepository<HotelAmenity> HotelAmenities { get; }
    public IRepository<Booking> Bookings { get; }
    public IRepository<Message> Messages { get; }
    public IRepository<AuditLog> AuditLogs { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        
        Hotels = new HotelRepository(_context);
        HotelImages = new Repository<HotelImage>(_context);
        Services = new Repository<Service>(_context);
        ServiceImages = new Repository<ServiceImage>(_context);
        Categories = new Repository<Category>(_context);
        Amenities = new Repository<Amenity>(_context);
        HotelAmenities = new Repository<HotelAmenity>(_context);
        Bookings = new Repository<Booking>(_context);
        Messages = new Repository<Message>(_context);
        AuditLogs = new Repository<AuditLog>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
