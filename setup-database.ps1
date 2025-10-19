# EventHotelBroker Database Setup Script
# This script helps set up the database for the EventHotelBroker application

Write-Host "EventHotelBroker Database Setup" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""

# Check if .NET EF tools are installed
Write-Host "Checking for Entity Framework Core tools..." -ForegroundColor Yellow
$efInstalled = dotnet ef --version 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "Entity Framework Core tools not found. Installing..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
} else {
    Write-Host "Entity Framework Core tools found: $efInstalled" -ForegroundColor Green
}

Write-Host ""

# Navigate to project directory
$projectPath = "EventHotelBroker"
if (Test-Path $projectPath) {
    Set-Location $projectPath
    Write-Host "Changed directory to: $projectPath" -ForegroundColor Green
} else {
    Write-Host "Project directory not found: $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Create initial migration
Write-Host "Creating initial migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration created successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to create migration. Please check the error messages above." -ForegroundColor Red
    exit 1
}

Write-Host ""

# Apply migration to database
Write-Host "Applying migration to database..." -ForegroundColor Yellow
Write-Host "Make sure your MySQL server is running and the connection string in appsettings.json is correct." -ForegroundColor Cyan
Write-Host ""
Read-Host "Press Enter to continue or Ctrl+C to cancel"

dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Database updated successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Setup complete! You can now run the application with: dotnet run" -ForegroundColor Green
    Write-Host ""
    Write-Host "Default admin credentials:" -ForegroundColor Cyan
    Write-Host "  Email: admin@eventhotelbroker.com" -ForegroundColor White
    Write-Host "  Password: Admin@123" -ForegroundColor White
    Write-Host ""
    Write-Host "IMPORTANT: Change the admin password after first login!" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "Failed to update database. Please check:" -ForegroundColor Red
    Write-Host "  1. MySQL server is running" -ForegroundColor Yellow
    Write-Host "  2. Connection string in appsettings.json is correct" -ForegroundColor Yellow
    Write-Host "  3. Database exists or user has permission to create it" -ForegroundColor Yellow
    exit 1
}
