using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Error;

[AllowAnonymous]
[Route("Error")]
public class ErrorController : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => View("Index");
}
