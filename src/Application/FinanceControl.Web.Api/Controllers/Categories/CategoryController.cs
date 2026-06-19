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
    public async Task<IActionResult> Get([FromQuery] DataFilterDto filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1) filter.PageSize = 100;
        return Ok(await appService.FilterAsync(filter));
    }

    [HttpGet("{id:guid}", Name = "ObterCategoria")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await appService.GetByIdAsync(id);
        return dto == null ? NotFound("Categoria não encontrada.") : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CategoryRegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await appService.CreateAsync(dto);
        return CreatedAtRoute("ObterCategoria", new { id = created.CategoryId }, created);
    }

    [HttpPost("registerCategory")]
    public Task<IActionResult> RegisterCategory([FromBody] CategoryRegisterDto dto) =>
        Post(dto);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] CategoryUpdateDto dto)
    {
        if (id != dto.CategoryId) return BadRequest("Id inválido.");
        var updated = await appService.UpdateAsync(dto);
        return updated == null ? NotFound("Categoria não encontrada.") : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await appService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound("Categoria não encontrada.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
