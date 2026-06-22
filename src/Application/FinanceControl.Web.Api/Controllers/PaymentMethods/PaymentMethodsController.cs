using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Interfaces.AppServices.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.PaymentMethods;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodsController(IPaymentMethodAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false, [FromQuery] Guid? userId = null) =>
        Ok(await appService.ListAsync(activeOnly: !includeInactive, userId: userId));

    [HttpGet("{id:guid}", Name = "GetPaymentMethodById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await appService.GetByIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymentMethodCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await appService.CreateAsync(dto);
            return CreatedAtRoute("GetPaymentMethodById", new { id = created.PaymentMethodId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] PaymentMethodUpdateDto dto)
    {
        if (id != dto.PaymentMethodId) return BadRequest("Id inválido.");
        try
        {
            var updated = await appService.UpdateAsync(dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await appService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
