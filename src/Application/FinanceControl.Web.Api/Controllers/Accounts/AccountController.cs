using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.Accounts;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter, CancellationToken cancellationToken)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;

        if (Request.Query.TryGetValue("userId", out var userId))
        {
            filter.Filters ??= new Dictionary<string, string>();
            filter.Filters["userId"] = userId!;
        }

        var result = await appService.FilterAsync(filter, cancellationToken);
        return Ok(result.Result);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetAccountById")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await appService.GetByIdAsync(id, cancellationToken);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AccountCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await appService.CreateAsync(dto, cancellationToken);
            return CreatedAtRoute("GetAccountById", new { id = created.AccountId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put(int id, [FromBody] AccountUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.AccountId) return BadRequest();
        var updated = await appService.UpdateAsync(dto, cancellationToken);
        return updated == null ? NotFound() : NoContent();
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
            return BadRequest(new { message = ex.Message });
        }
    }
}
