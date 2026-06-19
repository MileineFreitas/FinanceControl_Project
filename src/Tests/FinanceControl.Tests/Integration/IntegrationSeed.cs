using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceControl.Tests.Integration;

internal static class IntegrationSeed
{
    public static async Task EnsureDatabaseAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public static async Task<(Guid UserId, Guid CategoryId, Guid AccountId, Guid PaymentMethodId)> EnsureUserAndCategoryAsync(IServiceProvider services)
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

        var paymentMethod = new PaymentMethod
        {
            Name = $"Meio_{suffix}",
            Icon = "💳",
            IsActive = true,
            DateCreated = DateTimeOffset.UtcNow
        };
        db.PaymentMethods.Add(paymentMethod);

        var account = new Account
        {
            Name = $"Conta_{suffix}",
            InitialBalance = 0,
            CurrentBalance = 0,
            CreatedAt = DateTime.UtcNow,
            UserId = user.UserId,
            IsActive = true
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        return (user.UserId, category.CategoryId, account.AccountId, paymentMethod.PaymentMethodId);
    }
}
