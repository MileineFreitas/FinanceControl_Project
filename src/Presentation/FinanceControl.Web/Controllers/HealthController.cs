using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers;

/// <summary>Endpoint simples para verificação de disponibilidade (monitoramento / load balancer).</summary>
[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(new { status = "healthy", service = "FinanceControl.Web", timestamp = DateTime.UtcNow });
}
