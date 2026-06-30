using Microsoft.AspNetCore.Localization;

namespace FinanceControl.Web.Helpers;

public static class CultureCookieHelper
{
    public static void SetCultureCookie(HttpResponse response, string culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
            return;

        response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false
            });
    }
}
