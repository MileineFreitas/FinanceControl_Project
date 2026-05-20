using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.API.Controllers.Accounts;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public AccountController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> Get([FromQuery] int? userId)
    {
        var query = _context.Accounts.AsNoTracking().AsQueryable();
        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId || a.UserId == null);

        return Ok(await query.OrderBy(a => a.Name).ToListAsync());
    }

    [HttpGet("{id:int}", Name = "GetAccountById")]
    public async Task<ActionResult<Account>> GetById(int id)
    {
        var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountId == id);
        if (account == null)
            return NotFound();
        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> Post([FromBody] AccountCreateDto dto)
    {
        var now = DateTime.UtcNow;
        var entity = new Account
        {
            Name = dto.Name,
            InitialBalance = dto.InitialBalance,
            CurrentBalance = dto.InitialBalance,
            CreatedAt = now,
            UserId = dto.UserId
        };
        _context.Accounts.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtRoute("GetAccountById", new { id = entity.AccountId }, entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] AccountUpdateDto dto)
    {
        if (id != dto.AccountId)
            return BadRequest();

        var entity = await _context.Accounts.FindAsync(id);
        if (entity == null)
            return NotFound();

        entity.Name = dto.Name;
        entity.InitialBalance = dto.InitialBalance;
        entity.CurrentBalance = dto.CurrentBalance;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (id == 1)
            return BadRequest("A conta padrão (Principal) não pode ser excluída.");

        var hasTx = await _context.Transactions.AnyAsync(t => t.AccountId == id);
        if (hasTx)
            return BadRequest("Não é possível excluir: existem transações vinculadas a esta conta.");

        var entity = await _context.Accounts.FindAsync(id);
        if (entity == null)
            return NotFound();

        _context.Accounts.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
