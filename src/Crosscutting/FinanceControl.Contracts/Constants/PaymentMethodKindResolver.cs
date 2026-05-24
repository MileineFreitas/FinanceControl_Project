using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Constants;

public static class PaymentMethodKindResolver
{
    public static PaymentKind? FromName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return name.Trim().ToLowerInvariant() switch
        {
            "débito" or "debito" => PaymentKind.Debit,
            "crédito" or "credito" => PaymentKind.Credit,
            "dinheiro" => PaymentKind.Cash,
            _ => null
        };
    }
}
