using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Interfaces.AppServices.TransactionTypes;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.TransactionTypes;

[Route("api/[controller]")]
[ApiController]
public class TransactionTypesController(ITransactionTypeAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default) =>
        Ok(await appService.ListAsync(activeOnly: !includeInactive, cancellationToken));

    [HttpGet("{id:int:min(1)}", Name = "GetTransactionTypeById")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await appService.GetByIdAsync(id, cancellationToken);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TransactionTypeCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await appService.CreateAsync(dto, cancellationToken);
            return CreatedAtRoute("GetTransactionTypeById", new { id = created.TransactionTypeId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put(int id, [FromBody] TransactionTypeUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.TransactionTypeId) return BadRequest("Id inválido.");
        try
        {
            var updated = await appService.UpdateAsync(dto, cancellationToken);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await appService.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
