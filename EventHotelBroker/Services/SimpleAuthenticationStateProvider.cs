using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EventHotelBroker.Services;

public class SimpleAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public SimpleAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userInfoResult = await _sessionStorage.GetAsync<UserInfo>("userInfo");
            
            if (userInfoResult.Success && userInfoResult.Value != null)
            {
                var userInfo = userInfoResult.Value;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                    new Claim(ClaimTypes.Name, userInfo.FullName),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.Role, userInfo.Role),
                    new Claim("Role", userInfo.Role)
                };

                if (!string.IsNullOrEmpty(userInfo.BusinessName))
                {
                    claims.Add(new Claim("BusinessName", userInfo.BusinessName));
                }

                var identity = new ClaimsIdentity(claims, "SimpleAuth");
                var user = new ClaimsPrincipal(identity);
                
                return new AuthenticationState(user);
            }
        }
        catch
        {
            // If there's an error reading from session, return anonymous
        }

        return new AuthenticationState(_anonymous);
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
