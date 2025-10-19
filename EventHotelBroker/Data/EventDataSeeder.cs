using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Data;

public static class EventDataSeeder
{
    public static async Task SeedEventDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Check if event data already exists
        if (await context.EventEquipments.AnyAsync())
        {
            return; // Data already seeded
        }

        // Get a sample provider (use first user or create one)
        var provider = await context.AspNetUsers.FirstOrDefaultAsync();
        if (provider == null)
        {
            return; // No users exist yet
        }

        var providerId = provider.Id;

        // Seed Event Equipment
        var equipments = new List<EventEquipment>
        {
            // Tents
            new EventEquipment
            {
                Name = "White Wedding Tent 20x30",
                Description = "Elegant white wedding tent perfect for outdoor ceremonies. Includes sidewalls and lighting setup. Weather-resistant and professionally installed.",
                Category = "Tents",
                PricePerUnit = 800.00m,
                AvailableQuantity = 5,
                Unit = "piece",
                Specifications = "Dimensions: 20x30 feet, Capacity: 150 guests, Material: High-quality canvas",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventEquipment
            {
                Name = "Party Canopy Tent 10x10",
                Description = "Compact canopy tent ideal for small gatherings, food stations, or registration areas. Easy setup and portable.",
                Category = "Tents",
                PricePerUnit = 150.00m,
                AvailableQuantity = 15,
                Unit = "piece",
                Specifications = "Dimensions: 10x10 feet, Pop-up design, UV protection",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Chairs
            new EventEquipment
            {
                Name = "Elegant Chiavari Chairs",
                Description = "Classic gold chiavari chairs perfect for weddings and formal events. Includes cushioned seats for comfort.",
                Category = "Chairs",
                PricePerUnit = 8.00m,
                AvailableQuantity = 300,
                Unit = "piece",
                Specifications = "Material: Resin, Color: Gold, Weight capacity: 250 lbs",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventEquipment
            {
                Name = "Folding Banquet Chairs",
                Description = "Comfortable padded folding chairs suitable for conferences and corporate events. Stackable for easy storage.",
                Category = "Chairs",
                PricePerUnit = 5.00m,
                AvailableQuantity = 500,
                Unit = "piece",
                Specifications = "Material: Metal frame with padded seat, Color: Black",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Sound Systems
            new EventEquipment
            {
                Name = "Professional PA Sound System",
                Description = "Complete professional sound system with speakers, mixer, microphones, and cables. Perfect for large events up to 500 guests.",
                Category = "Sound System",
                PricePerUnit = 500.00m,
                AvailableQuantity = 8,
                Unit = "set",
                Specifications = "2x 1000W speakers, 16-channel mixer, 4 wireless microphones, all cables included",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventEquipment
            {
                Name = "Wireless Microphone Set",
                Description = "Professional wireless microphone system with 4 handheld mics. Clear sound quality for speeches and presentations.",
                Category = "Sound System",
                PricePerUnit = 150.00m,
                AvailableQuantity = 12,
                Unit = "set",
                Specifications = "4 wireless handheld mics, receiver unit, rechargeable batteries",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Lighting
            new EventEquipment
            {
                Name = "LED Uplighting Package",
                Description = "Set of 12 LED uplights with wireless DMX control. Create stunning ambiance with customizable colors.",
                Category = "Lighting",
                PricePerUnit = 400.00m,
                AvailableQuantity = 10,
                Unit = "set",
                Specifications = "12 LED uplights, wireless controller, RGB color mixing, battery powered",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventEquipment
            {
                Name = "String Light Canopy",
                Description = "Beautiful bistro string lights perfect for creating a romantic atmosphere. 100 feet of warm white LED lights.",
                Category = "Lighting",
                PricePerUnit = 200.00m,
                AvailableQuantity = 20,
                Unit = "set",
                Specifications = "100 feet total length, warm white LEDs, weatherproof, includes mounting hardware",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Catering
            new EventEquipment
            {
                Name = "Buffet Table Set",
                Description = "Complete buffet setup with 8-foot tables, tablecloths, and chafing dishes. Perfect for serving large groups.",
                Category = "Catering",
                PricePerUnit = 250.00m,
                AvailableQuantity = 15,
                Unit = "set",
                Specifications = "2x 8-foot tables, white tablecloths, 6 chafing dishes with fuel",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventEquipment
            {
                Name = "Bar Station Package",
                Description = "Professional bar setup including bar counter, shelving, and accessories. Includes glassware for 100 guests.",
                Category = "Catering",
                PricePerUnit = 350.00m,
                AvailableQuantity = 8,
                Unit = "set",
                Specifications = "6-foot bar counter, back bar shelving, ice bins, glassware set",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Decoration
            new EventEquipment
            {
                Name = "Floral Centerpiece Collection",
                Description = "Set of 10 elegant floral centerpieces with vases. Fresh seasonal flowers arranged by professional florists.",
                Category = "Decoration",
                PricePerUnit = 400.00m,
                AvailableQuantity = 20,
                Unit = "set",
                Specifications = "10 centerpieces, glass vases included, seasonal flowers, delivered fresh",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventEquipment
            {
                Name = "Backdrop Draping Kit",
                Description = "Elegant fabric draping for backdrop or ceiling decoration. Includes pipe and drape system with sheer fabric.",
                Category = "Decoration",
                PricePerUnit = 300.00m,
                AvailableQuantity = 12,
                Unit = "set",
                Specifications = "20 feet wide x 10 feet tall, white sheer fabric, adjustable pipe system",
                ProviderId = providerId,
                IsAvailable = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.EventEquipments.AddRangeAsync(equipments);
        await context.SaveChangesAsync();

        // Seed Event Packages
        var packages = new List<EventPackage>
        {
            new EventPackage
            {
                Name = "Premium Wedding Package",
                Description = "Complete wedding package including tent, seating for 200, professional sound system, romantic lighting, and elegant decorations. Everything you need for your dream wedding!",
                PackageType = "Wedding",
                TotalPrice = 8500.00m,
                DiscountedPrice = 7500.00m,
                MinGuests = 150,
                MaxGuests = 200,
                Features = "[\"White Wedding Tent 20x30\", \"300 Chiavari Chairs\", \"Professional PA System\", \"LED Uplighting\", \"String Lights\", \"Floral Centerpieces\", \"Backdrop Draping\"]",
                ProviderId = providerId,
                IsActive = true,
                IsApproved = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventPackage
            {
                Name = "Corporate Conference Package",
                Description = "Professional conference setup with comfortable seating, state-of-the-art sound system, and presentation equipment. Perfect for business meetings and seminars.",
                PackageType = "Corporate",
                TotalPrice = 3500.00m,
                DiscountedPrice = 3000.00m,
                MinGuests = 50,
                MaxGuests = 150,
                Features = "[\"Banquet Chairs for 150\", \"Professional Sound System\", \"Wireless Microphones\", \"LED Lighting\", \"Registration Tables\"]",
                ProviderId = providerId,
                IsActive = true,
                IsApproved = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventPackage
            {
                Name = "Birthday Party Celebration",
                Description = "Fun and festive birthday party package with tent, seating, sound system, and decorations. Make your celebration memorable!",
                PackageType = "Birthday",
                TotalPrice = 2500.00m,
                DiscountedPrice = 2200.00m,
                MinGuests = 30,
                MaxGuests = 100,
                Features = "[\"Party Canopy Tents\", \"100 Folding Chairs\", \"Sound System\", \"String Lights\", \"Decorative Elements\"]",
                ProviderId = providerId,
                IsActive = true,
                IsApproved = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventPackage
            {
                Name = "Garden Wedding Package",
                Description = "Intimate garden wedding setup perfect for smaller ceremonies. Includes elegant seating, lighting, and beautiful floral arrangements.",
                PackageType = "Wedding",
                TotalPrice = 4500.00m,
                DiscountedPrice = 4000.00m,
                MinGuests = 50,
                MaxGuests = 100,
                Features = "[\"Party Canopy Tents\", \"150 Chiavari Chairs\", \"String Lights\", \"Floral Centerpieces\", \"Sound System\"]",
                ProviderId = providerId,
                IsActive = true,
                IsApproved = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventPackage
            {
                Name = "Executive Meeting Package",
                Description = "Sophisticated setup for executive meetings and board presentations. Includes premium seating and professional audio-visual equipment.",
                PackageType = "Conference",
                TotalPrice = 2000.00m,
                DiscountedPrice = null,
                MinGuests = 20,
                MaxGuests = 50,
                Features = "[\"50 Banquet Chairs\", \"Wireless Microphone Set\", \"LED Lighting\", \"Conference Tables\"]",
                ProviderId = providerId,
                IsActive = true,
                IsApproved = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new EventPackage
            {
                Name = "Outdoor Party Extravaganza",
                Description = "Ultimate outdoor party package with everything needed for an unforgettable celebration. Includes tent, seating, entertainment setup, and catering equipment.",
                PackageType = "Party",
                TotalPrice = 5500.00m,
                DiscountedPrice = 5000.00m,
                MinGuests = 100,
                MaxGuests = 200,
                Features = "[\"Large Party Tent\", \"200 Chairs\", \"Professional Sound System\", \"LED Uplighting\", \"Bar Station\", \"Buffet Tables\"]",
                ProviderId = providerId,
                IsActive = true,
                IsApproved = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.EventPackages.AddRangeAsync(packages);
        await context.SaveChangesAsync();

        // Seed Sample Event Bookings
        var bookings = new List<EventBooking>
        {
            new EventBooking
            {
                UserId = providerId,
                PackageId = packages[0].Id,
                EventName = "Sarah & Michael's Wedding",
                EventType = "Wedding",
                EventDate = DateTime.UtcNow.AddDays(60),
                EventEndDate = null,
                Venue = "Sunset Gardens, 123 Garden Lane, Beverly Hills, CA",
                ExpectedGuests = 180,
                TotalAmount = 7500.00m,
                DepositAmount = 2500.00m,
                Status = EventBookingStatus.Confirmed,
                SpecialRequests = "Please ensure the tent is set up by 2 PM. We need extra time for decoration setup.",
                ContactName = "Sarah Johnson",
                ContactPhone = "+1-555-0123",
                ContactEmail = "sarah.johnson@email.com",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new EventBooking
            {
                UserId = providerId,
                PackageId = packages[1].Id,
                EventName = "TechCorp Annual Conference 2025",
                EventType = "Corporate",
                EventDate = DateTime.UtcNow.AddDays(30),
                EventEndDate = DateTime.UtcNow.AddDays(32),
                Venue = "Grand Convention Center, 456 Business Blvd, Los Angeles, CA",
                ExpectedGuests = 120,
                TotalAmount = 3000.00m,
                DepositAmount = 1000.00m,
                Status = EventBookingStatus.Confirmed,
                SpecialRequests = "Need additional microphones for panel discussions. Require technical support on-site.",
                ContactName = "Robert Chen",
                ContactPhone = "+1-555-0456",
                ContactEmail = "r.chen@techcorp.com",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new EventBooking
            {
                UserId = providerId,
                PackageId = packages[2].Id,
                EventName = "Emma's 10th Birthday Party",
                EventType = "Birthday",
                EventDate = DateTime.UtcNow.AddDays(15),
                EventEndDate = null,
                Venue = "Riverside Park, 789 Park Avenue, Santa Monica, CA",
                ExpectedGuests = 50,
                TotalAmount = 2200.00m,
                DepositAmount = 700.00m,
                Status = EventBookingStatus.Pending,
                SpecialRequests = "Kid-friendly setup needed. Please include extra decorations in pink and purple colors.",
                ContactName = "Jennifer Martinez",
                ContactPhone = "+1-555-0789",
                ContactEmail = "j.martinez@email.com",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new EventBooking
            {
                UserId = providerId,
                PackageId = null,
                EventName = "Community Fundraiser Gala",
                EventType = "Party",
                EventDate = DateTime.UtcNow.AddDays(45),
                EventEndDate = null,
                Venue = "City Hall Ballroom, 321 Main Street, Pasadena, CA",
                ExpectedGuests = 200,
                TotalAmount = 4500.00m,
                DepositAmount = 1500.00m,
                Status = EventBookingStatus.Pending,
                SpecialRequests = "This is a charity event. Need elegant setup with stage area for presentations and auction.",
                ContactName = "David Thompson",
                ContactPhone = "+1-555-0321",
                ContactEmail = "d.thompson@community.org",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new EventBooking
            {
                UserId = providerId,
                PackageId = packages[5].Id,
                EventName = "Summer Company Picnic",
                EventType = "Party",
                EventDate = DateTime.UtcNow.AddDays(90),
                EventEndDate = null,
                Venue = "Lakeside Park, 555 Lake Drive, Malibu, CA",
                ExpectedGuests = 150,
                TotalAmount = 5000.00m,
                DepositAmount = null,
                Status = EventBookingStatus.Pending,
                SpecialRequests = "Outdoor event with games and activities. Need weather backup plan.",
                ContactName = "Lisa Anderson",
                ContactPhone = "+1-555-0654",
                ContactEmail = "l.anderson@company.com",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        await context.EventBookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Event sample data seeded successfully!");
        Console.WriteLine($"   - {equipments.Count} equipment items");
        Console.WriteLine($"   - {packages.Count} event packages");
        Console.WriteLine($"   - {bookings.Count} sample bookings");
    }
}
