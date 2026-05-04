using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.API.Controllers.TransactionTypes;

[Route("api/[controller]")]
[ApiController]
public class TransactionTypesController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public TransactionTypesController(FinanceDbContext context)
    {
        _context = context;
    }

    /// <summary>Lista os tipos (RECEITA / DESPESA). Somente leitura — dados fixos no banco.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionTypeDefinition>>> Get()
    {
        var list = await _context.TransactionTypes.AsNoTracking()
            .OrderBy(t => t.TransactionTypeId)
            .ToListAsync();
        return Ok(list);
    }
}
