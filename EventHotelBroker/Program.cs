using EventHotelBroker.Components;
using EventHotelBroker.Data;
using EventHotelBroker.Models;
using EventHotelBroker.Repositories;
using EventHotelBroker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure MySQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=eventhotelbroker;User=root;Password=;";

var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Register repositories and services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Add SignalR for real-time messaging
builder.Services.AddSignalR();

// Add API Controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "EventHotelBroker API", 
        Version = "v1",
        Description = "API for EventHotelBroker marketplace application"
    });
});

// Add CORS for API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Apply pending migrations
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
        
        // Seed data
        logger.LogInformation("Seeding database...");
        await DbSeeder.SeedAsync(services);
        await SampleDataSeeder.SeedSampleDataAsync(services);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw; // Re-throw to prevent app from starting with a broken database
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventHotelBroker API V1");
        c.RoutePrefix = "api/docs"; // Access Swagger at /api/docs
    });
}

// Enable CORS
app.UseCors("AllowAll");

// Map API Controllers
app.MapControllers();

// Map Blazor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
