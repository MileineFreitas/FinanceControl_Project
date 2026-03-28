using System.Threading.Tasks;
using FinanceControl.Domain.Entities.Transactions;
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
    public async Task<ActionResult> Get()
    {
        var transactions = await _context.Transactions.ToListAsync();
        if (transactions == null)
        {
            return BadRequest("Transações não encontradas...");
        }
        return Ok(transactions);
    }

    [HttpPost(Name = "NewTrasaction")]
    public ActionResult Post([FromBody] Transaction transaction)
    {
        if (transaction == null)
        {
            return BadRequest("Transação inválida...");
        }

        _context.Transactions.Add(transaction);
        _context.SaveChanges();
        return Ok("Transação cadastrada com sucesso...");
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var transaction = _context.Transactions.FirstOrDefault(c => c.TransactionId == id);
        if (transaction == null)
        {
            return NotFound("Transação não encontrada...");
        }

        _context.Transactions.Remove(transaction);
        _context.SaveChanges();
        return Ok("Transaçao removida com sucesso...");
    }
}
