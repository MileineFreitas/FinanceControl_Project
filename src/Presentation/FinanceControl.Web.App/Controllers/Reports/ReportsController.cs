using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Reports;

[Route("relatorios")]
public class ReportsController : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => RedirectToAction(nameof(PorMeioPagamento));

    [HttpGet("por-meio-pagamento")]
    public IActionResult PorMeioPagamento() => View("PorMeioPagamento");

    [HttpGet("por-categoria")]
    public IActionResult PorCategoria() => View("PorCategoria");

    [HttpGet("por-transacoes")]
    public IActionResult PorTransacoes() => View("PorTransacoes");
}
