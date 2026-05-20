using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Account;

[Route("conta")]
public class AccountController : Controller
{
    [HttpGet("configuracao")]
    public IActionResult Configuracao() => View("Configuration");

    [HttpGet("privacidade")]
    public IActionResult Privacidade() => View("Privacy");
}
