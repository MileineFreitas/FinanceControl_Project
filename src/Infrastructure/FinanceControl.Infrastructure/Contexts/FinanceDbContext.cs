using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Contexts;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
}

