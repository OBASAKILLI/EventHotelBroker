using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace EventHotelBroker.Utils
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        // In-memory token storage - survives SignalR connections where HttpContext.Session is null
        private string? _currentToken;
        private Task<AuthenticationState>? _hostAuthenticationStateTask;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            _hostAuthenticationStateTask = authenticationStateTask;
            NotifyAuthenticationStateChanged(authenticationStateTask);
        }

        // Public property so pages can read the current token
        public string? CurrentToken => _currentToken ?? _httpContextAccessor.HttpContext?.Session?.GetString("JWToken");

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // 1. Try in-memory token explicitly set in this circuit (e.g. from Login component)
                if (!string.IsNullOrEmpty(_currentToken))
                {
                    var claims = ParseClaimsFromJwt(_currentToken);
                    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role));
                    return new AuthenticationState(authenticatedUser);
                }

                // 2. Fall back to host environment authentication state (passed by ASP.NET Core SignalR hub from HTTP connection)
                if (_hostAuthenticationStateTask != null)
                {
                    var hostState = await _hostAuthenticationStateTask;
                    if (hostState.User.Identity?.IsAuthenticated == true)
                    {
                        return hostState;
                    }
                }
                
                // 3. Fall back to session (available during initial HTTP request)
                string? token = null;
                try
                {
                    token = _httpContextAccessor.HttpContext?.Session?.GetString("JWToken");
                }
                catch { }
                
                // 4. Fall back to cookie (set by JS during login)
                if (string.IsNullOrEmpty(token))
                {
                    try
                    {
                        _httpContextAccessor.HttpContext?.Request?.Cookies?.TryGetValue("JWToken", out token);
                    }
                    catch { }
                }

                // 5. Fall back to HttpContext.User directly if authenticated by JwtBearer
                if (string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var httpUser = _httpContextAccessor.HttpContext?.User;
                        if (httpUser?.Identity?.IsAuthenticated == true)
                        {
                            return new AuthenticationState(httpUser);
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(token))
                {
                    if (token.Contains('%'))
                    {
                        try { token = Uri.UnescapeDataString(token); } catch { }
                    }

                    // Cache in memory so it survives the HTTP → SignalR transition
                    _currentToken = token;
                    
                    var claims = ParseClaimsFromJwt(token);
                    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role));
                    return new AuthenticationState(authenticatedUser);
                }

                return new AuthenticationState(_anonymous);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Exception: {ex.Message}");
                return new AuthenticationState(_anonymous);
            }
        }

        public Task MarkUserAsAuthenticated(string token)
        {
            // Store token in memory (critical for Blazor Server SignalR connections)
            _currentToken = token;
            
            // Also try to store in session if available and response has not started
            try
            {
                var ctx = _httpContextAccessor.HttpContext;
                if (ctx != null && !ctx.Response.HasStarted)
                {
                    var session = ctx.Session;
                    session?.SetString("JWToken", token);
                }
            }
            catch { /* Session/Response may not be available or editable during SignalR */ }

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt", ClaimTypes.Name, ClaimTypes.Role));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            _hostAuthenticationStateTask = authState;
            NotifyAuthenticationStateChanged(authState);
            return Task.CompletedTask;
        }

        public Task MarkUserAsLoggedOut()
        {
            _currentToken = null;
            _hostAuthenticationStateTask = null;
            
            try
            {
                var ctx = _httpContextAccessor.HttpContext;
                if (ctx != null && !ctx.Response.HasStarted)
                {
                    ctx.Session?.Clear();
                }
            }
            catch { /* Session/Response may not be available during SignalR */ }
            
            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
            return Task.CompletedTask;
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
                if (keyValuePairs == null) return claims;

                object? userId = null;
                keyValuePairs.TryGetValue(ClaimTypes.Name, out userId);
                if (userId == null) keyValuePairs.TryGetValue("unique_name", out userId);
                
                object? givenName = null;
                keyValuePairs.TryGetValue(ClaimTypes.GivenName, out givenName);
                if (givenName == null) keyValuePairs.TryGetValue("given_name", out givenName);
                
                object? email = null;
                keyValuePairs.TryGetValue(ClaimTypes.Email, out email);
                if (email == null) keyValuePairs.TryGetValue("email", out email);

                object? accountType = null;
                keyValuePairs.TryGetValue("AccountType", out accountType);

                if (userId != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, userId.ToString()!));
                    claims.Add(new Claim(ClaimTypes.GivenName, givenName?.ToString() ?? ""));
                    claims.Add(new Claim(ClaimTypes.Email, email?.ToString() ?? ""));
                    claims.Add(new Claim("AccountType", accountType?.ToString() ?? ""));
                }

                // Parse and populate standard ClaimTypes.Role claims
                var roleSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                void AddRole(string? r)
                {
                    if (string.IsNullOrWhiteSpace(r)) return;
                    foreach (var part in r.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    {
                        roleSet.Add(part);
                    }
                }

                // Check role claim from JWT (could be single string or array)
                if (keyValuePairs.TryGetValue(ClaimTypes.Role, out var roleObj) || keyValuePairs.TryGetValue("role", out roleObj))
                {
                    if (roleObj is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                        {
                            AddRole(item.GetString());
                        }
                    }
                    else
                    {
                        AddRole(roleObj?.ToString());
                    }
                }

                // Also check AccountType
                AddRole(accountType?.ToString());

                // Fallback: check if Name claim contains comma-separated roles
                if (userId != null)
                {
                    var partsList = userId.ToString()!.Split(',');
                    if (partsList.Length > 1)
                    {
                        AddRole(partsList[1]);
                    }
                }

                if (roleSet.Count == 0)
                {
                    roleSet.Add("User");
                }

                foreach (var r in roleSet)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] Exception parsing JWT: {ex.Message}");
            }
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
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
