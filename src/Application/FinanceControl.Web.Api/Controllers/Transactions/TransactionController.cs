using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Enums;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.API.Controllers.Transactions;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public TransactionController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> Get([FromQuery] int? userId, [FromQuery] int? accountId)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Account)
            .Include(t => t.TransactionTypeDefinition)
            .AsNoTracking()
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);
        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        var list = await query.OrderByDescending(t => t.Date).ThenByDescending(t => t.TransactionId).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}", Name = "GetTransactionById")]
    public async Task<ActionResult<Transaction>> GetById(int id)
    {
        var entity = await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Account)
            .Include(t => t.TransactionTypeDefinition)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId == id);

        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> Post([FromBody] TransactionCreateDto dto)
    {
        if (!await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId))
            return BadRequest(new { message = $"Não existe categoria com CategoryId={dto.CategoryId}. Cadastre categorias (ou reinicie a API para aplicar o seed) antes de lançar transações." });

        if (!await _context.Accounts.AnyAsync(a => a.AccountId == dto.AccountId))
            return BadRequest(new { message = $"Conta AccountId={dto.AccountId} não encontrada." });

        if (!await _context.Users.AnyAsync(u => u.UserId == dto.UserId))
            return BadRequest(new { message = $"Utilizador UserId={dto.UserId} não encontrado." });

        var now = DateTime.UtcNow;
        var entity = new Transaction
        {
            TransactionDescription = dto.TransactionDescription,
            TransactionValue = dto.TransactionValue,
            Date = dto.Date,
            TransactionTypeId = dto.TransactionTypeId,
            CategoryId = dto.CategoryId,
            AccountId = dto.AccountId,
            UserId = dto.UserId,
            Status = dto.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Transactions.Add(entity);
        await _context.SaveChangesAsync();

        if (entity.Status == TransactionStatus.Pago)
            await AdjustAccountBalanceAsync(entity.AccountId, entity.TransactionValue, entity.TransactionTypeId, subtractDelta: false);

        return CreatedAtRoute("GetTransactionById", new { id = entity.TransactionId }, entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] TransactionUpdateDto dto)
    {
        if (id != dto.TransactionId)
            return BadRequest();

        var entity = await _context.Transactions.FindAsync(id);
        if (entity == null)
            return NotFound();

        var oldAccountId = entity.AccountId;
        var oldValue = entity.TransactionValue;
        var oldTypeId = entity.TransactionTypeId;
        var oldStatus = entity.Status;

        if (oldStatus == TransactionStatus.Pago)
            await AdjustAccountBalanceAsync(oldAccountId, oldValue, oldTypeId, subtractDelta: true);

        entity.TransactionDescription = dto.TransactionDescription;
        entity.TransactionValue = dto.TransactionValue;
        entity.Date = dto.Date;
        entity.TransactionTypeId = dto.TransactionTypeId;
        entity.CategoryId = dto.CategoryId;
        entity.AccountId = dto.AccountId;
        entity.Status = dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (entity.Status == TransactionStatus.Pago)
            await AdjustAccountBalanceAsync(entity.AccountId, entity.TransactionValue, entity.TransactionTypeId, subtractDelta: false);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _context.Transactions.FindAsync(id);
        if (entity == null)
            return NotFound();

        if (entity.Status == TransactionStatus.Pago)
            await AdjustAccountBalanceAsync(entity.AccountId, entity.TransactionValue, entity.TransactionTypeId, subtractDelta: true);

        _context.Transactions.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Receita (tipo 1) aumenta saldo; despesa (tipo 2) diminui. subtractDelta=true remove o efeito de uma transação já contabilizada.</summary>
    private async Task AdjustAccountBalanceAsync(int accountId, decimal value, int transactionTypeId, bool subtractDelta)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            return;

        var delta = transactionTypeId == 1 ? value : -value;
        account.CurrentBalance += subtractDelta ? -delta : delta;
        await _context.SaveChangesAsync();
    }
}
