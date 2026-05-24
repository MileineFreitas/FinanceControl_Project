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
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;

        if (Request.Query.TryGetValue("userId", out var userId))
        {
            filter.Filters ??= new Dictionary<string, string>();
            filter.Filters["userId"] = userId!;
        }

        var result = await appService.FilterAsync(filter);
        return Ok(result.Result);
    }

    [HttpGet("{id:guid}", Name = "GetAccountById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await appService.GetByIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AccountCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await appService.CreateAsync(dto);
            return CreatedAtRoute("GetAccountById", new { id = created.AccountId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] AccountUpdateDto dto)
    {
        if (id != dto.AccountId) return BadRequest();
        var updated = await appService.UpdateAsync(dto);
        return updated == null ? NotFound() : NoContent();
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
            return BadRequest(new { message = ex.Message });
        }
    }
}
