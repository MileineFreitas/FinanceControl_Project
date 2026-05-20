using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Profile;

[Route("perfil")]
public class ProfileController : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => View("Index");
}
