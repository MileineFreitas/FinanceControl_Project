using System.Security.Claims;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Web.Helpers;

public static class CurrentUserExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var guid) ? guid : null;
    }

    public static string? GetUserEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email);

    public static string? GetUserDisplayName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name);

    public static UserFinancialPreferencesDto GetFinancialPreferences(this ClaimsPrincipal user) =>
        new()
        {
            Moeda = user.FindFirstValue(AuthClaimTypes.Currency) ?? FinancialPreferenceDefaults.Moeda,
            Idioma = user.FindFirstValue(AuthClaimTypes.Language) ?? FinancialPreferenceDefaults.Idioma,
            FormatoData = user.FindFirstValue(AuthClaimTypes.DateFormat) ?? FinancialPreferenceDefaults.FormatoData,
            InicioMes = int.TryParse(user.FindFirstValue(AuthClaimTypes.FinancialMonthStartDay), out var day)
                ? day
                : FinancialPreferenceDefaults.InicioMes
        };
}
