using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Error;

[Route("Error")]
public class ErrorController : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => View("Index");
}
