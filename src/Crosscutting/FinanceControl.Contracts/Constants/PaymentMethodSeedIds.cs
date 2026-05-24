namespace FinanceControl.Contracts.Constants;

/// <summary>Identificadores fixos dos meios de pagamento padrão do sistema.</summary>
public static class PaymentMethodSeedIds
{
    public static readonly Guid Debito = Guid.Parse("b1000001-0001-4001-8001-000000000001");
    public static readonly Guid Credito = Guid.Parse("b1000001-0001-4001-8001-000000000002");
    public static readonly Guid Dinheiro = Guid.Parse("b1000001-0001-4001-8001-000000000003");

    public static bool IsSystem(Guid paymentMethodId) =>
        paymentMethodId is var id && (id == Debito || id == Credito || id == Dinheiro);
}
