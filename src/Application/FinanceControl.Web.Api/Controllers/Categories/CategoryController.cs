using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceControl.Domain.Entities;

namespace FinanceControl.API.Controllers.Categories;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public CategoryController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> Get()
    {
        var categories = await _context.Categories
            .Include(c => c.User)
            .Include(c => c.TransactionTypeDefinition)
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public async Task<ActionResult<Category>> Get(int id)
    {
        var category = await _context.Categories
            .Include(c => c.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
        {
            return NotFound("Categoria não encontrada...");
        }
        return Ok(category);
    }

    [HttpPost]
    public ActionResult Post(Category category)
    {
        if (category == null)
        {
            return BadRequest();
        }

        _context.Categories.Add(category);
        _context.SaveChanges();
        return new CreatedAtRouteResult("ObterCategoria",
            new { id = category.CategoryId }, category);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Put(int id, [FromBody] CategoryUpdateDto dto)
    {
        if (id != dto.CategoryId)
            return BadRequest("Id invalido...");

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Categoria não encontrada...");

        category.CategoryName = dto.CategoryName;
        category.Description = dto.Description;
        category.TransactionTypeId = dto.TransactionTypeId;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category == null)
            return NotFound("Categoria não encontrada...");

        _context.Categories.Remove(category);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(category);
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Não é possível excluir: existem transações vinculadas a esta categoria." });
        }
    }

    [HttpPost("registerCategory")]
    public ActionResult CreateCategory([FromBody] CategoryRegisterDto dto)
    {
        if (dto == null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = _context.Users.OrderBy(u => u.UserId).Select(u => (int?)u.UserId).FirstOrDefault();

        var category = new Category
        {
            CategoryName = dto.CategoryName,
            Description = dto.CategoryDescription,
            TransactionTypeId = (int)dto.Type,
            DateCreated = DateTime.UtcNow,
            UserId = userId
        };

        _context.Categories.Add(category);
        _context.SaveChanges();
        return new CreatedAtRouteResult("ObterCategoria",
            new { id = category.CategoryId },
            new
            {
                category.CategoryId,
                category.CategoryName,
                category.Description,
                category.TransactionTypeId,
                category.DateCreated
            });
    }
}
