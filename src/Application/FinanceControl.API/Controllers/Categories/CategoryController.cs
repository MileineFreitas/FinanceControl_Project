using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceControl.Domain.Entities;

namespace FinanceControl.API.Controllers.Categories;

[Route("[controller]")]
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
            .Where(c => c.CategoryId <= 30)
            .AsNoTracking()
            .ToListAsync();

        if (categories == null)
        {
            return BadRequest("Categorias não encontradas...");
        }
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
    public ActionResult Put(int id, Category category)
    {
        if (id != category.CategoryId)
        {
            return BadRequest("Id invalido...");
        }

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();
        return Ok(category);
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return NotFound("Categoria não encontrada...");
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return Ok(category);
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
        var category = new Category
        {
            CategoryName = dto.CategoryName,
            Description = dto.CategoryDescription,
            Type = dto.Type,
            DateCreated = DateTime.UtcNow
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
                category.Type,
                category.DateCreated
            });
    }
}
