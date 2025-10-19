using EventHotelBroker.Models.DTOs;
using EventHotelBroker.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventHotelBroker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISimpleAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    // Hardcoded credentials
    private static readonly Dictionary<string, UserCredential> _users = new()
    {
        // Admin
        { "admin@eventhotelbroker.com", new UserCredential 
            { 
                Email = "admin@eventhotelbroker.com", 
                Password = "Admin@123", 
                FullName = "System Administrator", 
                Role = "Admin",
                UserId = "admin-001"
            } 
        },
        
        // Hotel Owners
        { "owner1@test.com", new UserCredential 
            { 
                Email = "owner1@test.com", 
                Password = "Owner@123", 
                FullName = "John Smith", 
                Role = "HotelOwner",
                BusinessName = "Smith Hotels Ltd",
                UserId = "owner-001"
            } 
        },
        { "owner2@test.com", new UserCredential 
            { 
                Email = "owner2@test.com", 
                Password = "Owner@123", 
                FullName = "Mary Johnson", 
                Role = "HotelOwner",
                BusinessName = "Coastal Resorts Kenya",
                UserId = "owner-002"
            } 
        },
        { "owner3@test.com", new UserCredential 
            { 
                Email = "owner3@test.com", 
                Password = "Owner@123", 
                FullName = "David Kimani", 
                Role = "HotelOwner",
                BusinessName = "Mountain View Lodges",
                UserId = "owner-003"
            } 
        },
        
        // Regular Users
        { "user1@test.com", new UserCredential 
            { 
                Email = "user1@test.com", 
                Password = "User@123", 
                FullName = "Jane Doe", 
                Role = "User",
                UserId = "user-001"
            } 
        },
        { "user2@test.com", new UserCredential 
            { 
                Email = "user2@test.com", 
                Password = "User@123", 
                FullName = "Peter Omondi", 
                Role = "User",
                UserId = "user-002"
            } 
        }
    };

    public AuthController(
        ISimpleAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - simple authentication with hardcoded credentials
    /// </summary>
    [HttpPost("login")]
    public ActionResult<AuthResponseDto> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Check if user exists and password matches
            if (_users.TryGetValue(request.Email.ToLower(), out var user))
            {
                if (user.Password == request.Password)
                {
                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "Login successful",
                        User = new UserDto
                        {
                            Id = user.UserId,
                            Email = user.Email,
                            FullName = user.FullName,
                            Role = user.Role,
                            BusinessName = user.BusinessName,
                            IsActive = true,
                            IsOwnerVerified = true
                        }
                    });
                }
            }
            
            return Ok(new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during login"
            });
        }
    }

    /// <summary>
    /// Register endpoint - disabled
    /// </summary>
    [HttpPost("register")]
    public ActionResult<AuthResponseDto> Register([FromBody] RegisterRequestDto request)
    {
        return Ok(new AuthResponseDto
        {
            Success = false,
            Message = "Registration is disabled. Please use pre-configured test accounts."
        });
    }

    /// <summary>
    /// Get available test accounts
    /// </summary>
    [HttpGet("test-accounts")]
    public ActionResult<object> GetTestAccounts()
    {
        var accounts = _users.Values.Select(u => new
        {
            u.Email,
            u.FullName,
            u.Role,
            u.BusinessName,
            Password = "***" // Don't expose actual passwords in production
        });

        return Ok(new
        {
            Message = "Available test accounts. Use these credentials to login.",
            Accounts = accounts
        });
    }
}
