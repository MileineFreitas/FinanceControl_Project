using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.Transactions;

[Route("api/[controller]")]
[ApiController]
public class TransactionController(ITransactionAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;

        if (Request.Query.TryGetValue("userId", out var userId))
        {
            filter.Filters ??= new Dictionary<string, string>();
            filter.Filters["userId"] = userId!;
        }

        if (Request.Query.TryGetValue("accountId", out var accountId))
        {
            filter.Filters ??= new Dictionary<string, string>();
            filter.Filters["accountId"] = accountId!;
        }

        return Ok(await appService.FilterAsync(filter));
    }

    [HttpGet("{id:int:min(1)}", Name = "GetTransactionById")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await appService.GetByIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TransactionCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await appService.CreateAsync(dto);
            return CreatedAtRoute("GetTransactionById", new { id = created.TransactionId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put(int id, [FromBody] TransactionUpdateDto dto)
    {
        if (id != dto.TransactionId) return BadRequest();
        try
        {
            var updated = await appService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await appService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
