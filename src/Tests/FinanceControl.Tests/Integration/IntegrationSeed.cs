using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceControl.Tests.Integration;

internal static class IntegrationSeed
{
    public static async Task EnsureDatabaseAndSeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        await db.Database.EnsureCreatedAsync();
        FinanceDbContextSeed.EnsureDemoUserAccountAndCategories(db);
    }

    public static async Task<(Guid UserId, Guid CategoryId)> EnsureUserAndCategoryAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        await db.Database.EnsureCreatedAsync();

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var user = new User
        {
            UserName = $"Test_{suffix}",
            UserEmail = $"{suffix}@test.local",
            Password = "Senha1234!",
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var category = new Category
        {
            CategoryName = $"Categoria_{suffix}",
            Description = "Seed teste",
            DateCreated = DateTimeOffset.UtcNow,
            UserId = user.UserId,
            IsActive = true
        };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        if (!await db.Accounts.AnyAsync(a => a.AccountId == SeedIds.DefaultAccount))
        {
            db.Accounts.Add(new Account
            {
                AccountId = SeedIds.DefaultAccount,
                Name = "Principal",
                InitialBalance = 0,
                CurrentBalance = 0,
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId,
                IsActive = true
            });
            await db.SaveChangesAsync();
        }

        return (user.UserId, category.CategoryId);
    }
}
