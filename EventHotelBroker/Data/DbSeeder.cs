using EventHotelBroker.Models;

namespace EventHotelBroker.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Hotel", Slug = "hotel", Description = "Hotels and lodging facilities" },
                new Category { Name = "Tent", Slug = "tent", Description = "Event tents and marquees" },
                new Category { Name = "Catering", Slug = "catering", Description = "Food and beverage services" },
                new Category { Name = "Sound & AV", Slug = "sound-av", Description = "Audio visual equipment" },
                new Category { Name = "Decor", Slug = "decor", Description = "Event decoration services" },
                new Category { Name = "Photography", Slug = "photography", Description = "Photography and videography" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Amenities
        if (!context.Amenities.Any())
        {
            var amenities = new List<Amenity>
            {
                new Amenity { Name = "Free WiFi", Slug = "free-wifi", Icon = "wifi" },
                new Amenity { Name = "Parking", Slug = "parking", Icon = "car" },
                new Amenity { Name = "Air Conditioning", Slug = "air-conditioning", Icon = "wind" },
                new Amenity { Name = "Swimming Pool", Slug = "swimming-pool", Icon = "waves" },
                new Amenity { Name = "Restaurant", Slug = "restaurant", Icon = "utensils" },
                new Amenity { Name = "Bar", Slug = "bar", Icon = "glass-martini" },
                new Amenity { Name = "Gym", Slug = "gym", Icon = "dumbbell" },
                new Amenity { Name = "Spa", Slug = "spa", Icon = "spa" },
                new Amenity { Name = "Conference Room", Slug = "conference-room", Icon = "presentation" },
                new Amenity { Name = "Garden", Slug = "garden", Icon = "tree" },
                new Amenity { Name = "Pet Friendly", Slug = "pet-friendly", Icon = "paw" },
                new Amenity { Name = "24/7 Security", Slug = "security", Icon = "shield" }
            };

            context.Amenities.AddRange(amenities);
            await context.SaveChangesAsync();
        }

        // Note: Hotels, Services, Messages, AuditLogs, and Bookings are seeded by SampleDataSeeder
        // to avoid foreign key issues with user references
    }
}
