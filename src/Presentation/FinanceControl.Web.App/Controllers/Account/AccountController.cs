using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Account;

[Route("conta")]
public class AccountController : Controller
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

    [HttpGet("privacidade")]
    public IActionResult Privacidade() => View("Privacy");
}
