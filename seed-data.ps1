# EventHotelBroker Database Seeding Script
# This script helps reset and seed the database with sample data

Write-Host "EventHotelBroker Database Seeding Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check if MySQL is accessible
Write-Host "Checking MySQL connection..." -ForegroundColor Yellow
$mysqlPath = "mysql"

try {
    $null = & $mysqlPath --version 2>&1
    Write-Host "✓ MySQL found" -ForegroundColor Green
} catch {
    Write-Host "✗ MySQL not found in PATH. Please ensure MySQL is installed and accessible." -ForegroundColor Red
    exit 1
}

# Prompt for MySQL credentials
Write-Host ""
$mysqlUser = Read-Host "Enter MySQL username (default: root)"
if ([string]::IsNullOrWhiteSpace($mysqlUser)) {
    $mysqlUser = "root"
}

$mysqlPassword = Read-Host "Enter MySQL password (leave empty if no password)" -AsSecureString
$mysqlPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($mysqlPassword)
)

# Ask if user wants to reset the database
Write-Host ""
$reset = Read-Host "Do you want to DROP and recreate the database? (y/N)"

if ($reset -eq "y" -or $reset -eq "Y") {
    Write-Host ""
    Write-Host "Dropping and recreating database..." -ForegroundColor Yellow
    
    $dropDbScript = @"
DROP DATABASE IF EXISTS eventhotelbroker;
CREATE DATABASE eventhotelbroker CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
"@
    
    if ([string]::IsNullOrWhiteSpace($mysqlPasswordPlain)) {
        $dropDbScript | & $mysqlPath -u $mysqlUser
    } else {
        $dropDbScript | & $mysqlPath -u $mysqlUser -p"$mysqlPasswordPlain"
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database recreated successfully" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to recreate database" -ForegroundColor Red
        exit 1
    }
    
    # Run migrations
    Write-Host ""
    Write-Host "Running Entity Framework migrations..." -ForegroundColor Yellow
    Set-Location -Path "EventHotelBroker"
    
    dotnet ef database update
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Migrations applied successfully" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to apply migrations" -ForegroundColor Red
        Set-Location -Path ".."
        exit 1
    }
    
    Set-Location -Path ".."
}

# Run the application to seed data
Write-Host ""
Write-Host "Starting application to seed sample data..." -ForegroundColor Yellow
Write-Host "The application will start and automatically seed the database." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the application after seeding is complete." -ForegroundColor Cyan
Write-Host ""

Set-Location -Path "EventHotelBroker"
dotnet run

Set-Location -Path ".."

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Database seeding process completed!" -ForegroundColor Green
Write-Host ""
Write-Host "You can now login with these credentials:" -ForegroundColor Cyan
Write-Host "  Admin: admin@eventhotelbroker.com / Admin@123" -ForegroundColor White
Write-Host "  Owner: owner1@test.com / Owner@123" -ForegroundColor White
Write-Host "  User:  user1@test.com / User@123" -ForegroundColor White
Write-Host ""
Write-Host "See SAMPLE_DATA.md for complete details on seeded data." -ForegroundColor Cyan
