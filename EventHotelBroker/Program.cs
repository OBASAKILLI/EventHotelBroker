using EventHotelBroker.Components;
using EventHotelBroker.Data;
using EventHotelBroker.Models;
using EventHotelBroker.Repositories;
using EventHotelBroker.Services;
using Microsoft.AspNetCore.Components.Authorization;
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
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<EventHotelBroker.Services.IEmailService, EventHotelBroker.Services.EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMpesaPaymentService, MpesaPaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ToastService>();

// Session & Authentication
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<EventHotelBroker.TokenProvider>();
builder.Services.AddScoped<EventHotelBroker.Utils.CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<EventHotelBroker.Utils.CustomAuthenticationStateProvider>());

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
        
        // Align migration history with existing database tables if __EFMigrationsHistory was missing or incomplete
        await EnsureMigrationsHistoryAlignedAsync(context, logger);

        // Apply pending migrations
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
        
        // Seed essential data (admin account only)
        logger.LogInformation("Seeding database...");
        await DbSeeder.SeedAsync(services);
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
app.UseSession();

// Middleware: Read JWT token from cookie into session (bridges SignalR login → HTTP session)
app.Use(async (context, next) =>
{
    if (context.Request.Cookies.TryGetValue("JWToken", out var token) && !string.IsNullOrEmpty(token))
    {
        var sessionToken = context.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(sessionToken))
        {
            context.Session.SetString("JWToken", token);
            await context.Session.CommitAsync();
        }
    }
    await next();
});

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

// Map lightweight endpoint to keep server alive (SmarterASP Scheduled Task)
app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();

static async Task EnsureMigrationsHistoryAlignedAsync(ApplicationDbContext context, ILogger logger)
{
    try
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
                    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
                    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
                    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
                ) CHARACTER SET=utf8mb4;";
            await cmd.ExecuteNonQueryAsync();
        }

        async Task<bool> TableExistsAsync(string table)
        {
            using var c = connection.CreateCommand();
            c.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t";
            var p = c.CreateParameter();
            p.ParameterName = "@t";
            p.Value = table;
            c.Parameters.Add(p);
            return Convert.ToInt32(await c.ExecuteScalarAsync()) > 0;
        }

        async Task<bool> ColumnExistsAsync(string table, string column)
        {
            using var c = connection.CreateCommand();
            c.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t AND COLUMN_NAME = @col";
            var p1 = c.CreateParameter();
            p1.ParameterName = "@t";
            p1.Value = table;
            c.Parameters.Add(p1);
            var p2 = c.CreateParameter();
            p2.ParameterName = "@col";
            p2.Value = column;
            c.Parameters.Add(p2);
            return Convert.ToInt32(await c.ExecuteScalarAsync()) > 0;
        }

        async Task RecordMigrationAsync(string migrationId)
        {
            using var c = connection.CreateCommand();
            c.CommandText = "INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES (@m, '8.0.0')";
            var p = c.CreateParameter();
            p.ParameterName = "@m";
            p.Value = migrationId;
            c.Parameters.Add(p);
            await c.ExecuteNonQueryAsync();
        }

        if (await TableExistsAsync("Amenities"))
            await RecordMigrationAsync("20251018174255_InitialCreate");

        if (await TableExistsAsync("EventPackages"))
            await RecordMigrationAsync("20251019080002_AddEventManagement");

        if (await ColumnExistsAsync("EventPackages", "IsCustomizable"))
            await RecordMigrationAsync("20251019190728_AddIsCustomizableToEventPackage");

        if (await TableExistsAsync("Users"))
        {
            await RecordMigrationAsync("20251029161314_AddPasswordHashToUser");
            await RecordMigrationAsync("20260324204832_AddUserRegistrationFields");
        }

        if (await ColumnExistsAsync("Bookings", "RejectionReason"))
            await RecordMigrationAsync("20260330092613_AddBookingRejectionFields");

        if (await ColumnExistsAsync("Hotels", "Category"))
            await RecordMigrationAsync("20260511105312_AddCategoryToHotels");

        if (await ColumnExistsAsync("Hotels", "Phone"))
            await RecordMigrationAsync("20260511111011_AddPhoneToHotels");

        if (await ColumnExistsAsync("Hotels", "Email"))
            await RecordMigrationAsync("20260511131916_AddEmailToHotels");

        if (await TableExistsAsync("Reviews"))
            await RecordMigrationAsync("20260720094344_AddReviewsTable");

        logger.LogInformation("Database migration history validated and aligned with existing tables.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Could not check migration history alignment against existing database tables.");
    }
}
