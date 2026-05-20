using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.API.Controllers.Categories;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryAppService appService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter, CancellationToken cancellationToken)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;
        return Ok(await appService.FilterAsync(filter, cancellationToken));
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var dto = await appService.GetByIdAsync(id, cancellationToken);
        return dto == null ? NotFound("Categoria não encontrada.") : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CategoryRegisterDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await appService.CreateAsync(dto, cancellationToken);
        return CreatedAtRoute("ObterCategoria", new { id = created.CategoryId }, created);
    }

    [HttpPost("registerCategory")]
    public Task<IActionResult> RegisterCategory([FromBody] CategoryRegisterDto dto, CancellationToken cancellationToken) =>
        Post(dto, cancellationToken);

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put(int id, [FromBody] CategoryUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.CategoryId) return BadRequest("Id inválido.");
        var updated = await appService.UpdateAsync(dto, cancellationToken);
        return updated == null ? NotFound("Categoria não encontrada.") : Ok(updated);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await appService.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound("Categoria não encontrada.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
