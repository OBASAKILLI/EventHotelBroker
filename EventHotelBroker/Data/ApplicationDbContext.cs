using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> AspNetUsers { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<HotelImage> HotelImages { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceImage> ServiceImages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<HotelAmenity> HotelAmenities { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.BusinessName).HasMaxLength(200);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(100);
        });

        // Hotel configuration
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.IsPublished);
            entity.HasIndex(e => e.Slug).IsUnique();
            
            entity.Property(e => e.PricePerNight).HasPrecision(12, 2);
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);

            entity.HasOne(e => e.Owner)
                .WithMany(u => u.Hotels)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // HotelImage configuration
        modelBuilder.Entity<HotelImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.HotelId);

            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Images)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Service configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProviderId);
            entity.HasIndex(e => e.CategoryId);
            
            entity.Property(e => e.Price).HasPrecision(12, 2);

            entity.HasOne(e => e.Provider)
                .WithMany(u => u.Services)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ServiceImage configuration
        modelBuilder.Entity<ServiceImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ServiceId);

            entity.HasOne(e => e.Service)
                .WithMany(s => s.Images)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Amenity configuration
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // HotelAmenity (many-to-many) configuration
        modelBuilder.Entity<HotelAmenity>(entity =>
        {
            entity.HasKey(e => new { e.HotelId, e.AmenityId });

            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.HotelAmenities)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Amenity)
                .WithMany(a => a.HotelAmenities)
                .HasForeignKey(e => e.AmenityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.HotelId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Bookings)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Message configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => e.ReceiverId);
            entity.HasIndex(e => e.HotelId);

            entity.HasOne(e => e.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(e => e.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
