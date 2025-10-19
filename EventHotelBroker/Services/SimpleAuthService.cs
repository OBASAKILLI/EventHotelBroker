using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EventHotelBroker.Services;

public interface ISimpleAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
}

public class SimpleAuthService : ISimpleAuthService
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly SimpleAuthenticationStateProvider _authStateProvider;
    
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

    public SimpleAuthService(
        ProtectedSessionStorage sessionStorage,
        SimpleAuthenticationStateProvider authStateProvider)
    {
        _sessionStorage = sessionStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        // Check if user exists and password matches
        if (_users.TryGetValue(email.ToLower(), out var user))
        {
            if (user.Password == password)
            {
                // Store user info in session
                var userInfo = new UserInfo
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    BusinessName = user.BusinessName
                };
                
                await _sessionStorage.SetAsync("userInfo", userInfo);
                _authStateProvider.NotifyAuthenticationStateChanged();
                
                return new AuthResult { Success = true, Message = "Login successful", User = userInfo };
            }
        }
        
        return new AuthResult { Success = false, Message = "Invalid email or password" };
    }

    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync("userInfo");
        _authStateProvider.NotifyAuthenticationStateChanged();
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<UserInfo>("userInfo");
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }
}

public class UserCredential
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
}

public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserInfo? User { get; set; }
}
