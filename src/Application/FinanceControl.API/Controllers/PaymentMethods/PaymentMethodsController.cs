using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.API.Controllers.PaymentMethods;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodsController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public PaymentMethodsController(FinanceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethod>>> Get(CancellationToken cancellationToken)
    {
        var list = await _context.PaymentMethods.AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}", Name = "ObterMeioPagamento")]
    public async Task<ActionResult<PaymentMethod>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.PaymentMethods.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentMethodId == id, cancellationToken);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpPost("registerPaymentMethod")]
    public ActionResult RegisterPaymentMethod([FromBody] PaymentMethodRegisterDto dto)
    {
        if (dto == null)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new PaymentMethod
        {
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DateCreated = DateTime.UtcNow
        };

        _context.PaymentMethods.Add(entity);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterMeioPagamento",
            new { id = entity.PaymentMethodId },
            entity);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == id, cancellationToken);
        if (entity == null)
            return NotFound();
        _context.PaymentMethods.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
