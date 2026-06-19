using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Dashboards;

[Route("dashboards")]
public class DashboardsController : Controller
{
    [HttpGet("geral")]
    public IActionResult Geral() => View("Geral");

    [HttpGet("por-categoria")]
    public IActionResult PorCategoria() => View("PorCategoria");

    [HttpGet("por-transacoes")]
    public IActionResult PorTransacoes() => View("PorTransacoes");
}
