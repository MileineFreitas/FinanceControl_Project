using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinanceControl.Infrastructure.Seeding;

public static class DatabaseMigrationExtensions
{
    public static void ApplyMigrations(this IServiceProvider services, ILogger? logger = null)
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
    }
}
