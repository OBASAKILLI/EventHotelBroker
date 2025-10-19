using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Data;

public static class SampleDataSeeder
{
    public static async Task SeedSampleDataAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Check if sample data already exists
        if (context.Hotels.Any())
        {
            return; // Sample data already seeded
        }

        // Create sample hotel owners (these match the hardcoded credentials)
        var owner1 = new ApplicationUser
        {
            Id = "owner-001",
            Email = "owner1@test.com",
            FullName = "John Smith",
            PhoneNumber = "+254712345678",
            Role = "HotelOwner",
            BusinessName = "Smith Hotels Ltd",
            RegistrationNumber = "BN123456",
            IsActive = true,
            IsOwnerVerified = true
        };

        var owner2 = new ApplicationUser
        {
            Id = "owner-002",
            Email = "owner2@test.com",
            FullName = "Mary Johnson",
            PhoneNumber = "+254723456789",
            Role = "HotelOwner",
            BusinessName = "Coastal Resorts Kenya",
            RegistrationNumber = "BN789012",
            IsActive = true,
            IsOwnerVerified = true
        };

        var owner3 = new ApplicationUser
        {
            Id = "owner-003",
            Email = "owner3@test.com",
            FullName = "David Kimani",
            PhoneNumber = "+254734567890",
            Role = "HotelOwner",
            BusinessName = "Mountain View Lodges",
            RegistrationNumber = "BN345678",
            IsActive = true,
            IsOwnerVerified = true
        };

        // Create sample regular users
        var user1 = new ApplicationUser
        {
            Id = "user-001",
            Email = "user1@test.com",
            FullName = "Jane Doe",
            PhoneNumber = "+254745678901",
            Role = "User",
            IsActive = true
        };

        var user2 = new ApplicationUser
        {
            Id = "user-002",
            Email = "user2@test.com",
            FullName = "Peter Omondi",
            PhoneNumber = "+254756789012",
            Role = "User",
            IsActive = true
        };

        // Add users to database (required for foreign key constraints)
        // Using raw SQL to include ASP.NET Identity required columns
        if (!context.AspNetUsers.Any())
        {
            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO AspNetUsers (Id, Email, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, Role, IsActive, IsOwnerVerified, BusinessName, RegistrationNumber, CreatedAt)
                VALUES 
                ('owner-001', 'owner1@test.com', 0, '+254712345678', 0, 0, 0, 0, 'John Smith', 'HotelOwner', 1, 1, 'Smith Hotels Ltd', 'BN123456', UTC_TIMESTAMP()),
                ('owner-002', 'owner2@test.com', 0, '+254723456789', 0, 0, 0, 0, 'Mary Johnson', 'HotelOwner', 1, 1, 'Coastal Resorts Kenya', 'BN789012', UTC_TIMESTAMP()),
                ('owner-003', 'owner3@test.com', 0, '+254734567890', 0, 0, 0, 0, 'David Kimani', 'HotelOwner', 1, 1, 'Mountain View Lodges', 'BN345678', UTC_TIMESTAMP()),
                ('user-001', 'user1@test.com', 0, '+254745678901', 0, 0, 0, 0, 'Jane Doe', 'User', 1, 0, NULL, NULL, UTC_TIMESTAMP()),
                ('user-002', 'user2@test.com', 0, '+254756789012', 0, 0, 0, 0, 'Peter Omondi', 'User', 1, 0, NULL, NULL, UTC_TIMESTAMP())
            ");
        }

        // Get amenities
        var amenities = context.Amenities.ToList();
        var wifiAmenity = amenities.FirstOrDefault(a => a.Slug == "free-wifi");
        var parkingAmenity = amenities.FirstOrDefault(a => a.Slug == "parking");
        var acAmenity = amenities.FirstOrDefault(a => a.Slug == "air-conditioning");
        var poolAmenity = amenities.FirstOrDefault(a => a.Slug == "swimming-pool");
        var restaurantAmenity = amenities.FirstOrDefault(a => a.Slug == "restaurant");
        var conferenceAmenity = amenities.FirstOrDefault(a => a.Slug == "conference-room");
        var gardenAmenity = amenities.FirstOrDefault(a => a.Slug == "garden");
        var securityAmenity = amenities.FirstOrDefault(a => a.Slug == "security");

        // Create sample hotels
        var hotel1 = new Hotel
        {
            OwnerId = owner1.Id,
            Name = "Grand Plaza Hotel Nairobi",
            Slug = "grand-plaza-hotel-nairobi",
            Description = "Luxurious 5-star hotel in the heart of Nairobi's business district. Perfect for corporate events, conferences, and weddings. Features state-of-the-art facilities, elegant ballrooms, and exceptional service.",
            Address = "Kenyatta Avenue, CBD",
            City = "Nairobi",
            Country = "Kenya",
            Latitude = -1.286389m,
            Longitude = 36.817223m,
            Capacity = 500,
            PricePerNight = 25000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };

        var hotel2 = new Hotel
        {
            OwnerId = owner2.Id,
            Name = "Serena Beach Resort & Spa",
            Slug = "serena-beach-resort-spa",
            Description = "Stunning beachfront resort on the pristine shores of the Indian Ocean. Ideal for destination weddings, beach parties, and corporate retreats. Offers water sports, spa services, and breathtaking ocean views.",
            Address = "Shanzu Beach Road",
            City = "Mombasa",
            Country = "Kenya",
            Latitude = -3.983333m,
            Longitude = 39.733333m,
            Capacity = 300,
            PricePerNight = 35000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        };

        var hotel3 = new Hotel
        {
            OwnerId = owner3.Id,
            Name = "Mount Kenya Safari Lodge",
            Slug = "mount-kenya-safari-lodge",
            Description = "Eco-friendly lodge nestled at the foothills of Mount Kenya. Perfect for nature retreats, team building events, and intimate gatherings. Experience wildlife, hiking trails, and stunning mountain views.",
            Address = "Nanyuki-Meru Highway",
            City = "Nanyuki",
            Country = "Kenya",
            Latitude = -0.016667m,
            Longitude = 37.066667m,
            Capacity = 150,
            PricePerNight = 18000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };

        var hotel4 = new Hotel
        {
            OwnerId = owner1.Id,
            Name = "Westlands Conference Center",
            Slug = "westlands-conference-center",
            Description = "Modern conference facility in Westlands with cutting-edge AV equipment. Specializes in corporate events, seminars, product launches, and business meetings. Flexible spaces for 50-1000 attendees.",
            Address = "Waiyaki Way, Westlands",
            City = "Nairobi",
            Country = "Kenya",
            Latitude = -1.267778m,
            Longitude = 36.806389m,
            Capacity = 1000,
            PricePerNight = 50000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        };

        var hotel5 = new Hotel
        {
            OwnerId = owner2.Id,
            Name = "Lake Nakuru View Hotel",
            Slug = "lake-nakuru-view-hotel",
            Description = "Charming hotel overlooking the famous Lake Nakuru National Park. Great for wildlife enthusiasts, small weddings, and weekend getaways. Offers game drives, bird watching, and serene lake views.",
            Address = "Flamingo Road",
            City = "Nakuru",
            Country = "Kenya",
            Latitude = -0.303056m,
            Longitude = 36.080278m,
            Capacity = 120,
            PricePerNight = 15000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var hotel6 = new Hotel
        {
            OwnerId = owner3.Id,
            Name = "Karen Blixen Garden Estate",
            Slug = "karen-blixen-garden-estate",
            Description = "Elegant colonial-style estate in the leafy suburbs of Karen. Perfect for garden weddings, cocktail parties, and intimate celebrations. Features manicured gardens, vintage architecture, and personalized service.",
            Address = "Karen Road",
            City = "Nairobi",
            Country = "Kenya",
            Latitude = -1.323333m,
            Longitude = 36.706944m,
            Capacity = 200,
            PricePerNight = 30000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = false, // Pending approval
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var hotel7 = new Hotel
        {
            OwnerId = owner1.Id,
            Name = "Diani Palms Beach Resort",
            Slug = "diani-palms-beach-resort",
            Description = "Tropical paradise on Diani Beach with white sand and crystal-clear waters. Ideal for beach weddings, honeymoons, and family events. Offers water sports, beach volleyball, and sunset cruises.",
            Address = "Diani Beach Road",
            City = "Diani",
            Country = "Kenya",
            Latitude = -4.283333m,
            Longitude = 39.566667m,
            Capacity = 250,
            PricePerNight = 28000,
            Currency = "KES",
            IsPublished = true,
            IsApproved = false, // Pending approval
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        context.Hotels.AddRange(hotel1, hotel2, hotel3, hotel4, hotel5, hotel6, hotel7);
        await context.SaveChangesAsync();

        // Add amenities to hotels
        if (wifiAmenity != null && parkingAmenity != null && acAmenity != null)
        {
            var hotelAmenities = new List<HotelAmenity>
            {
                // Hotel 1 - Grand Plaza
                new HotelAmenity { HotelId = hotel1.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel1.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel1.Id, AmenityId = acAmenity.Id },
                new HotelAmenity { HotelId = hotel1.Id, AmenityId = conferenceAmenity!.Id },
                new HotelAmenity { HotelId = hotel1.Id, AmenityId = restaurantAmenity!.Id },
                new HotelAmenity { HotelId = hotel1.Id, AmenityId = securityAmenity!.Id },

                // Hotel 2 - Serena Beach
                new HotelAmenity { HotelId = hotel2.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel2.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel2.Id, AmenityId = acAmenity.Id },
                new HotelAmenity { HotelId = hotel2.Id, AmenityId = poolAmenity!.Id },
                new HotelAmenity { HotelId = hotel2.Id, AmenityId = restaurantAmenity!.Id },

                // Hotel 3 - Mount Kenya Safari
                new HotelAmenity { HotelId = hotel3.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel3.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel3.Id, AmenityId = restaurantAmenity!.Id },
                new HotelAmenity { HotelId = hotel3.Id, AmenityId = gardenAmenity!.Id },

                // Hotel 4 - Westlands Conference
                new HotelAmenity { HotelId = hotel4.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel4.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel4.Id, AmenityId = acAmenity.Id },
                new HotelAmenity { HotelId = hotel4.Id, AmenityId = conferenceAmenity!.Id },
                new HotelAmenity { HotelId = hotel4.Id, AmenityId = securityAmenity!.Id },

                // Hotel 5 - Lake Nakuru
                new HotelAmenity { HotelId = hotel5.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel5.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel5.Id, AmenityId = restaurantAmenity!.Id },

                // Hotel 6 - Karen Blixen
                new HotelAmenity { HotelId = hotel6.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel6.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel6.Id, AmenityId = gardenAmenity!.Id },

                // Hotel 7 - Diani Palms
                new HotelAmenity { HotelId = hotel7.Id, AmenityId = wifiAmenity.Id },
                new HotelAmenity { HotelId = hotel7.Id, AmenityId = parkingAmenity.Id },
                new HotelAmenity { HotelId = hotel7.Id, AmenityId = poolAmenity!.Id },
                new HotelAmenity { HotelId = hotel7.Id, AmenityId = restaurantAmenity!.Id }
            };

            context.HotelAmenities.AddRange(hotelAmenities);
            await context.SaveChangesAsync();
        }

        // Create sample bookings
        var bookings = new List<Booking>
        {
            new Booking
            {
                UserId = user1.Id,
                HotelId = hotel1.Id,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(32),
                HeadCount = 150,
                Status = BookingStatus.Pending,
                Notes = "Corporate annual general meeting. Need projector and sound system.",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Booking
            {
                UserId = user2.Id,
                HotelId = hotel2.Id,
                StartDate = DateTime.UtcNow.AddDays(45),
                EndDate = DateTime.UtcNow.AddDays(48),
                HeadCount = 100,
                Status = BookingStatus.Confirmed,
                Notes = "Beach wedding ceremony and reception. Need setup for 100 guests.",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Booking
            {
                UserId = user1.Id,
                HotelId = hotel3.Id,
                StartDate = DateTime.UtcNow.AddDays(20),
                EndDate = DateTime.UtcNow.AddDays(22),
                HeadCount = 50,
                Status = BookingStatus.Confirmed,
                Notes = "Team building retreat for 50 employees.",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Booking
            {
                UserId = user2.Id,
                HotelId = hotel4.Id,
                StartDate = DateTime.UtcNow.AddDays(15),
                EndDate = DateTime.UtcNow.AddDays(15),
                HeadCount = 300,
                Status = BookingStatus.Pending,
                Notes = "Product launch event. Full day conference.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Booking
            {
                UserId = user1.Id,
                HotelId = hotel5.Id,
                StartDate = DateTime.UtcNow.AddDays(60),
                EndDate = DateTime.UtcNow.AddDays(62),
                HeadCount = 40,
                Status = BookingStatus.Pending,
                Notes = "Weekend getaway for family reunion.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        context.Bookings.AddRange(bookings);
        await context.SaveChangesAsync();

        Console.WriteLine("Sample data seeded successfully!");
    }
}
