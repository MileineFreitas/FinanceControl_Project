using System.Security.Claims;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Users;
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
            user.SecurityStamp,
            new UserFinancialPreferencesDto
            {
                Moeda = user.Moeda,
                Idioma = user.Idioma,
                FormatoData = user.FormatoData,
                InicioMes = user.InicioMes
            });

    public static Task SignInUserAsync(
        this HttpContext httpContext,
        Guid userId,
        string name,
        string email,
        Guid securityStamp,
        UserFinancialPreferencesDto? preferences = null)
    {
        preferences ??= new UserFinancialPreferencesDto();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(AuthClaimTypes.SecurityStamp, securityStamp.ToString()),
            new(AuthClaimTypes.Currency, preferences.Moeda),
            new(AuthClaimTypes.Language, preferences.Idioma),
            new(AuthClaimTypes.DateFormat, preferences.FormatoData),
            new(AuthClaimTypes.FinancialMonthStartDay, preferences.InicioMes.ToString()),
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

    public static Task RefreshFinancialPreferencesAsync(
        this HttpContext httpContext,
        UserFinancialPreferencesDto preferences)
    {
        var user = httpContext.User;
        var userId = user.GetUserId();
        if (userId == null)
            return Task.CompletedTask;

        var stampValue = user.FindFirstValue(AuthClaimTypes.SecurityStamp);
        if (!Guid.TryParse(stampValue, out var securityStamp))
            securityStamp = Guid.NewGuid();

        return httpContext.SignInUserAsync(
            userId.Value,
            user.GetUserDisplayName() ?? string.Empty,
            user.GetUserEmail() ?? string.Empty,
            securityStamp,
            preferences);
    }
}
