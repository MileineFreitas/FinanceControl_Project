using FinanceControl.Contracts.Dtos.Users;
using Microsoft.AspNetCore.Localization;
using System.Security.Claims;

namespace FinanceControl.Web.Helpers;

public sealed class UserPreferenceCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
            return NullProviderCultureResult;

        var language = user.FindFirstValue(AuthClaimTypes.Language);
        if (string.IsNullOrWhiteSpace(language) ||
            !FinancialPreferenceDefaults.IdiomasValidos.Contains(language))
            return NullProviderCultureResult;

        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(language, language));
    }
}
