using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FinanceControl.Web.Middleware;

public class SecurityStampValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserCliService userCli)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetUserId();
            var stampClaim = context.User.FindFirst(AuthClaimTypes.SecurityStamp)?.Value;

            if (userId == null || string.IsNullOrEmpty(stampClaim))
            {
                await SignOutAndRedirectAsync(context);
                return;
            }

            try
            {
                var currentStamp = await userCli.GetSecurityStampAsync(userId.Value);
                if (currentStamp == null ||
                    !string.Equals(currentStamp.Value.ToString(), stampClaim, StringComparison.OrdinalIgnoreCase))
                {
                    await SignOutAndRedirectAsync(context);
                    return;
                }
            }
            catch
            {
                // Mantém a sessão em falhas transitórias da API.
            }
        }

        await next(context);
    }

    private static async Task SignOutAndRedirectAsync(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (IsApiRequest(context))
            return;

        context.Response.Redirect("/login");
    }

    private static bool IsApiRequest(HttpContext context) =>
        context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
}
