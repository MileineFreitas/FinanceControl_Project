namespace FinanceControl.Infrastructure.Seeding;

/// <summary>Identificadores fixos para seed e regras de negócio (conta/tipos padrão).</summary>
public static class SeedIds
{
    public static readonly Guid DefaultAccount = Guid.Parse("a1000001-0001-4001-8001-000000000001");

    public static readonly Guid TransactionTypeDebito = Guid.Parse("b1000001-0001-4001-8001-000000000001");
    public static readonly Guid TransactionTypeCredito = Guid.Parse("b1000001-0001-4001-8001-000000000002");
    public static readonly Guid TransactionTypeDinheiro = Guid.Parse("b1000001-0001-4001-8001-000000000003");
}
