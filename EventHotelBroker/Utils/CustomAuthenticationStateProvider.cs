using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace EventHotelBroker.Utils
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        // In-memory token storage - survives SignalR connections where HttpContext.Session is null
        private string? _currentToken;

        public CustomAuthenticationStateProvider(IHttpContextAccessor HttpContextAccessor)
        {
            _httpContextAccessor = HttpContextAccessor;
        }

        // Public property so pages can read the current token
        public string? CurrentToken => _currentToken ?? _httpContextAccessor.HttpContext?.Session?.GetString("JWToken");

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // 1. Try in-memory token first (persists across SignalR)
                string? token = _currentToken;
                
                // 2. Fall back to session (available during initial HTTP request)
                if (string.IsNullOrEmpty(token))
                {
                    token = _httpContextAccessor.HttpContext?.Session?.GetString("JWToken");
                }
                
                // 3. Fall back to cookie (set by JS during login)
                if (string.IsNullOrEmpty(token))
                {
                    _httpContextAccessor.HttpContext?.Request?.Cookies?.TryGetValue("JWToken", out token);
                }

                if (!string.IsNullOrEmpty(token))
                {
                    // Cache in memory so it survives the HTTP → SignalR transition
                    _currentToken = token;
                    
                    var claims = ParseClaimsFromJwt(token);
                    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                    return Task.FromResult(new AuthenticationState(authenticatedUser));
                }
                else
                {
                    return Task.FromResult(new AuthenticationState(_anonymous));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Exception: {ex.Message}");
                return Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            // Store token in memory (critical for Blazor Server SignalR connections)
            _currentToken = token;
            
            // Also try to store in session if available
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.SetString("JWToken", token);
                    await session.CommitAsync();
                }
            }
            catch { /* Session may not be available during SignalR */ }

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            _currentToken = null;
            
            try
            {
                _httpContextAccessor.HttpContext?.Session?.Clear();
            }
            catch { /* Session may not be available */ }
            
            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
        }
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length < 2) return claims;

                byte[] jsonBytes = ParseBase64WithoutPadding(parts[1]);
                string decodedString = Encoding.UTF8.GetString(jsonBytes);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(decodedString);
                
                // JwtSecurityTokenHandler maps full ClaimTypes URLs to short JWT names:
                //   ClaimTypes.Name -> "unique_name"
                //   ClaimTypes.GivenName -> "given_name"  
                //   ClaimTypes.Email -> "email"
                // Try both full URL and short JWT name for each claim
                
                object userId = null;
                keyValuePairs.TryGetValue(ClaimTypes.Name, out userId);
                if (userId == null) keyValuePairs.TryGetValue("unique_name", out userId);
                
                object givenName = null;
                keyValuePairs.TryGetValue(ClaimTypes.GivenName, out givenName);
                if (givenName == null) keyValuePairs.TryGetValue("given_name", out givenName);
                
                object email = null;
                keyValuePairs.TryGetValue(ClaimTypes.Email, out email);
                if (email == null) keyValuePairs.TryGetValue("email", out email);

                object accountType = null;
                keyValuePairs.TryGetValue("AccountType", out accountType);

                if (userId != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, userId.ToString()));
                    claims.Add(new Claim(ClaimTypes.GivenName, givenName?.ToString() ?? ""));
                    claims.Add(new Claim(ClaimTypes.Email, email?.ToString() ?? ""));
                    claims.Add(new Claim("AccountType", accountType?.ToString() ?? ""));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Exception parsing JWT: {ex.Message}");
                throw;
            }
            return claims;
        }
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

    }
}
