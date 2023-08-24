using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ServerBlazorAuth.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _protectedSessionStorage;
    private ClaimsPrincipal _claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _protectedSessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorage = await _protectedSessionStorage.GetAsync<SessionStore>("UserLogInSession");

            var userSession = userSessionStorage.Success ? userSessionStorage.Value : null;

            if (userSession == null)
            {
                return await Task.FromResult(new AuthenticationState(_claimPrincipal));
            }
            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, userSession.UserName),
            new Claim(ClaimTypes.Role, userSession.Role)
        }, "BasicAuth"));
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_claimPrincipal));
        }  
    }

    public async Task UpdateUserAuthenticationState(SessionStore loggedSession)
    {
        ClaimsPrincipal claimsPrincipal;
        if(loggedSession != null) 
        {
            await _protectedSessionStorage.SetAsync("UserLogInSession", loggedSession);
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, loggedSession.UserName),
            new Claim(ClaimTypes.Role, loggedSession.Role)
        }));
        }
        else
        {
            await _protectedSessionStorage.DeleteAsync("UserLogInSession");
            claimsPrincipal = _claimPrincipal;
        }
         this.NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}
