namespace FinanceControl.Contracts.Enumerators.Transactions;

public static class PaymentKindExtensions
{
    public static string ToDisplayName(this PaymentKind kind) =>
        kind switch
        {
            PaymentKind.Debit => "Débito",
            PaymentKind.Credit => "Crédito",
            PaymentKind.Cash => "Dinheiro",
            _ => kind.ToString()
        };
}
