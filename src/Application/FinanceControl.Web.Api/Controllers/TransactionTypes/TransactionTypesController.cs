using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Interfaces.AppServices.TransactionTypes;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.TransactionTypes;

[Route("api/[controller]")]
[ApiController]
public class TransactionTypesController(ITransactionTypeAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false) =>
        Ok(await appService.ListAsync(activeOnly: !includeInactive));

    [HttpGet("{id:int:min(1)}", Name = "GetTransactionTypeById")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await appService.GetByIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TransactionTypeCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await appService.CreateAsync(dto);
            return CreatedAtRoute("GetTransactionTypeById", new { id = created.TransactionTypeId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put(int id, [FromBody] TransactionTypeUpdateDto dto)
    {
        if (id != dto.TransactionTypeId) return BadRequest("Id inválido.");
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

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete(int id)
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
