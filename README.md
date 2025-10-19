# EventHotelBroker

A marketplace web application that connects hotel/venue owners and service providers with event seekers and tourists.

## Features

### Three User Roles
- **Admin**: Full control over users, listings, approvals, and reports
- **HotelOwner/ServiceProvider**: Create and manage listings, respond to inquiries, view analytics
- **User/EventSeeker/Tourist**: Search listings, message owners, request bookings

### Core Functionality
- Hotel and service listing management
- Advanced search and filtering (location, date, capacity, price, amenities)
- Internal messaging system with SignalR
- Booking request workflow
- Admin approval system for listings
- Image upload and management
- Audit logging
- Email notifications

## Technology Stack

- **Framework**: ASP.NET Core 8.0 with Blazor Server
- **Database**: MySQL 8.0+ with Entity Framework Core
- **ORM**: Pomelo.EntityFrameworkCore.MySql
- **Authentication**: ASP.NET Identity
- **Real-time**: SignalR
- **Email**: MailKit
- **Image Processing**: SixLabors.ImageSharp

## Prerequisites

- .NET 8.0 SDK
- MySQL 8.0 or higher
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Database Setup

Create a MySQL database:

```sql
CREATE DATABASE eventhotelbroker CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 2. Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=eventhotelbroker;User=root;Password=your_password;"
  }
}
```

### 3. Install Dependencies

```bash
cd EventHotelBroker
dotnet restore
```

### 4. Run Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The application will be available at `https://localhost:5001`

## Default Admin Credentials

- **Email**: admin@eventhotelbroker.com
- **Password**: Admin@123

**Important**: Change the admin password after first login!

## Project Structure

```
EventHotelBroker/
├── Components/           # Blazor components
│   ├── Pages/           # Page components
│   ├── Layout/          # Layout components
│   └── Shared/          # Shared components
├── Data/                # Database context and seeder
├── Models/              # Domain models
├── Repositories/        # Data access layer
├── Services/            # Business logic layer
├── wwwroot/            # Static files
└── Program.cs          # Application entry point
```

## Database Schema

### Main Tables
- **AspNetUsers**: Extended Identity user with business fields
- **Hotels**: Hotel/venue listings
- **HotelImages**: Hotel images
- **Services**: Service provider listings
- **ServiceImages**: Service images
- **Categories**: Service categories
- **Amenities**: Hotel amenities
- **HotelAmenities**: Many-to-many relationship
- **Bookings**: Booking requests
- **Messages**: Internal messaging
- **AuditLogs**: System audit trail

## Development Roadmap

### Phase 1: MVP (Current)
- [x] Project setup and architecture
- [x] Database models and migrations
- [x] Repository and service layers
- [x] Identity and authentication
- [ ] Admin panel UI
- [ ] Hotel owner dashboard
- [ ] User search and booking
- [ ] Messaging system
- [ ] Email notifications

### Phase 2: Enhancements
- [ ] Payment integration (M-Pesa, Stripe)
- [ ] Advanced analytics dashboard
- [ ] Calendar availability management
- [ ] Review and rating system
- [ ] Mobile responsive design
- [ ] API for mobile apps

### Phase 3: Advanced Features
- [ ] Multi-vendor booking coordination
- [ ] Dynamic pricing
- [ ] AI-powered recommendations
- [ ] Multi-language support
- [ ] Advanced reporting

## Security Features

- Password hashing with ASP.NET Identity
- Role-based authorization
- CSRF protection (built-in)
- SQL injection prevention (parameterized queries)
- Input validation
- Rate limiting (to be added)
- Secure file uploads

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is proprietary software. All rights reserved.

## Support

For issues and questions, please contact the development team.
