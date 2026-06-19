namespace FinanceControl.Contracts.Enumerators.Transactions;

public static class TransactionTypeKindExtensions
{
    public static string ToDisplayName(this TransactionTypeKind kind) =>
        kind switch
        {
            TransactionTypeKind.Receita => "Receita",
            TransactionTypeKind.Despesa => "Despesa",
            _ => kind.ToString()
        };

    public static bool IsDefinedKind(this TransactionTypeKind kind) =>
        kind is TransactionTypeKind.Receita or TransactionTypeKind.Despesa;
}
