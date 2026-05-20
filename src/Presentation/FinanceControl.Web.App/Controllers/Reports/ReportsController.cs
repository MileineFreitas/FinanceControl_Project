using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Reports;

[Route("relatorios")]
public class ReportsController : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() =>
        RedirectToAction("Geral", "Dashboards");
}
