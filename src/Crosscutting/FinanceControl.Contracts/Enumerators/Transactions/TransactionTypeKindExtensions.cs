namespace FinanceControl.Contracts.Enumerators.Transactions;

public static class TransactionTypeKindExtensions
{
    public static bool IsDefinedKind(this TransactionTypeKind kind) =>
        kind is TransactionTypeKind.Receita or TransactionTypeKind.Despesa;
}
