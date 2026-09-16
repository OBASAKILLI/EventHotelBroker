using EventHotelBroker.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace EventHotelBroker.Utils
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenStore _tokenStore;

        public CustomAuthenticationStateProvider(
            IHttpContextAccessor httpContextAccessor,
            TokenStore tokenStore)
        {
            _httpContextAccessor  = httpContextAccessor;
            _tokenStore           = tokenStore;
        }

        // Stable browser-level key stored in an HTTP-only cookie
        private string ClientId
        {
            get
            {
                var ctx = _httpContextAccessor.HttpContext;
                if (ctx == null) return "anonymous";
                if (!ctx.Request.Cookies.TryGetValue("_cid", out var cid) || string.IsNullOrEmpty(cid))
                {
                    cid = Guid.NewGuid().ToString("N");
                    ctx.Response.Cookies.Append("_cid", cid, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure   = true,
                        SameSite = SameSiteMode.Lax,
                        Expires  = DateTimeOffset.UtcNow.AddDays(30)
                    });
                }
                return cid;
            }
        }

        public string? CurrentToken
        {
            get
            {
                // 1. Singleton store keyed by stable client ID
                var token = _tokenStore.Get(ClientId);
                // 2. Fall back to session (initial HTTP request)
                if (string.IsNullOrEmpty(token))
                    token = _httpContextAccessor.HttpContext?.Session?.GetString("JWToken");
                // 3. Fall back to JWT cookie
                if (string.IsNullOrEmpty(token))
                    _httpContextAccessor.HttpContext?.Request?.Cookies?.TryGetValue("JWToken", out token);
                return token;
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = CurrentToken;

                if (!string.IsNullOrEmpty(token))
                {
                    // Persist to the singleton store so all circuits for this browser share it
                    _tokenStore.Set(ClientId, token);

                    var claims = ParseClaimsFromJwt(token);
                    var user   = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                    return Task.FromResult(new AuthenticationState(user));
                }

                return Task.FromResult(new AuthenticationState(_anonymous));
            }
            catch
            {
                return Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            // Store in singleton (cross-circuit)
            _tokenStore.Set(ClientId, token);

            // Also persist in session when available
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.SetString("JWToken", token);
                    await session.CommitAsync();
                }
            }
            catch { /* Session unavailable during SignalR */ }

            var user      = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            _tokenStore.Remove(ClientId);

            try { _httpContextAccessor.HttpContext?.Session?.Clear(); }
            catch { }

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_anonymous)));

            await Task.CompletedTask;
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length < 2) return claims;

                byte[] jsonBytes = ParseBase64WithoutPadding(parts[1]);
                string decoded   = Encoding.UTF8.GetString(jsonBytes);
                var kv           = JsonSerializer.Deserialize<Dictionary<string, object>>(decoded);
                if (kv == null) return claims;

                kv.TryGetValue(ClaimTypes.Name,       out var userId);
                if (userId == null) kv.TryGetValue("unique_name", out userId);

                kv.TryGetValue(ClaimTypes.GivenName, out var givenName);
                if (givenName == null) kv.TryGetValue("given_name", out givenName);

                kv.TryGetValue(ClaimTypes.Email, out var email);
                if (email == null) kv.TryGetValue("email", out email);

                kv.TryGetValue("AccountType", out var accountType);

                if (userId != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name,      userId.ToString()!));
                    claims.Add(new Claim(ClaimTypes.GivenName, givenName?.ToString() ?? ""));
                    claims.Add(new Claim(ClaimTypes.Email,     email?.ToString()     ?? ""));
                    claims.Add(new Claim("AccountType",        accountType?.ToString() ?? ""));
                }
            }
            catch { /* Return empty claims on parse failure */ }
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "=";  break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
