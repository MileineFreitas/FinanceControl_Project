using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinanceControl.Infrastructure.Seeding;

public static class DatabaseMigrationExtensions
{
    /// <summary>Aplica migrations pendentes e popula dados iniciais (uma vez por base vazia).</summary>
    public static void ApplyMigrationsAndSeed(this IServiceProvider services, ILogger? logger = null)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        if (db.Database.IsRelational())
        {
            var pending = db.Database.GetPendingMigrations().ToList();
            if (pending.Count > 0)
            {
                logger?.LogInformation("Aplicando {Count} migration(s): {Names}", pending.Count, string.Join(", ", pending));
                db.Database.Migrate();
                logger?.LogInformation("Migrations aplicadas com sucesso.");
            }
            else
            {
                logger?.LogDebug("Nenhuma migration pendente.");
            }
        }
        else
        {
            db.Database.EnsureCreated();
            logger?.LogDebug("Base em memória criada (EnsureCreated).");
        }

        FinanceDbContextSeed.EnsureDemoUserAccountAndCategories(db);
        logger?.LogInformation("Seed de demonstração verificado.");
    }
}
