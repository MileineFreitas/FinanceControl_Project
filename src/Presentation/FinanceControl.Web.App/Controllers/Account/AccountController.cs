using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Account;

[Route("conta")]
public class AccountController(IUserCliService userCli) : Controller
{
    [HttpGet("configuracao")]
    public IActionResult Configuracao() => View("Configuration");

    [HttpPost("configuracao")]
    [ValidateAntiForgeryToken]
    public IActionResult Configuracao(IFormCollection form)
    {
        TempData["ConfigSucesso"] = "Configurações salvas com sucesso.";
        return RedirectToAction(nameof(Configuracao));
    }

    [HttpPost("encerrar-sessoes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EncerrarSessoes()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        try
        {
            var response = await userCli.RevokeOtherSessionsAsync(userId.Value);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ConfigErro"] = "Não foi possível encerrar as sessões. Tente novamente.";
                return RedirectToAction(nameof(Configuracao));
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LoginInfo"] = "Todas as sessões foram encerradas. Faça login novamente.";
            return RedirectToAction("Index", "Login");
        }
        catch (Exception)
        {
            TempData["ConfigErro"] = "Não foi possível encerrar as sessões. Tente novamente.";
        }

        return RedirectToAction(nameof(Configuracao));
    }

    [HttpPost("excluir-conta")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirConta()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        try
        {
            var response = await userCli.DeleteAsync(userId.Value);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ConfigErro"] = "Não foi possível excluir a conta. Tente novamente.";
                return RedirectToAction(nameof(Configuracao));
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LoginInfo"] = "Sua conta foi excluída permanentemente.";
            return RedirectToAction("Index", "Login");
        }
        catch (Exception)
        {
            TempData["ConfigErro"] = "Não foi possível excluir a conta. Tente novamente.";
        }

        return RedirectToAction(nameof(Configuracao));
    }

    [HttpGet("privacidade")]
    public IActionResult Privacidade() => View("Privacy");
}
