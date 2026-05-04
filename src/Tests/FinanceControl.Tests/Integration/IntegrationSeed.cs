using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Enums;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceControl.Tests.Integration;

internal static class IntegrationSeed
{
    /// <summary>Cria o modelo na BD em memória e aplica o seed do modelo (HasData: tipos de transação, conta padrão, etc.).</summary>
    public static async Task EnsureDatabaseAndSeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    /// <summary>Garante schema + seed do modelo (TransactionTypes, Accounts via HasData) e cria usuário + categoria.</summary>
    public static async Task<(int UserId, int CategoryId)> EnsureUserAndCategoryAsync(IServiceProvider services)
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
            UserType = UserType.User,
            DateCreated = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var category = new Category
        {
            CategoryName = $"Categoria_{suffix}",
            Description = "Seed teste",
            DateCreated = DateTime.UtcNow,
            TransactionTypeId = 2,
            UserId = user.UserId
        };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return (user.UserId, category.CategoryId);
    }
}
