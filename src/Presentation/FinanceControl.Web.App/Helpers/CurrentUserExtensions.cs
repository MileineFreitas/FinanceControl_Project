using System.Security.Claims;

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
}
