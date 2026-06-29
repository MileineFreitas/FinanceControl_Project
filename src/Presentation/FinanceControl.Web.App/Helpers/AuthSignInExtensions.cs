using System.Security.Claims;
using FinanceControl.Contracts.Dtos.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FinanceControl.Web.Helpers;

public static class AuthSignInExtensions
{
    public static Task SignInUserAsync(this HttpContext httpContext, LoginResponseDto user) =>
        httpContext.SignInUserAsync(
            user.UserId,
            user.Name,
            user.Email,
            user.SecurityStamp);

    public static Task SignInUserAsync(
        this HttpContext httpContext,
        Guid userId,
        string name,
        string email,
        Guid securityStamp)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(AuthClaimTypes.SecurityStamp, securityStamp.ToString()),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        return httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            });
    }
}
