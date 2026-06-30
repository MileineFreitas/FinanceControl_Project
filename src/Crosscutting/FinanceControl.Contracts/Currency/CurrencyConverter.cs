namespace FinanceControl.Contracts.Currency;

/// <summary>
/// Conversão entre moedas suportadas usando taxas de referência fixas (pivot BRL).
/// </summary>
public static class CurrencyConverter
{
    /// <summary>Quantos BRL equivalem a 1 unidade da moeda.</summary>
    private static readonly Dictionary<string, decimal> BrlPerUnit = new(StringComparer.OrdinalIgnoreCase)
    {
        ["BRL"] = 1m,
        ["USD"] = 5.50m,
        ["EUR"] = 6.00m
    };

    public static decimal Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        if (amount == 0m)
            return 0m;

        var from = Normalize(fromCurrency);
        var to = Normalize(toCurrency);

        if (from == to)
            return amount;

        if (!BrlPerUnit.TryGetValue(from, out var fromRate))
            throw new ArgumentException($"Moeda de origem não suportada: {fromCurrency}.", nameof(fromCurrency));

        if (!BrlPerUnit.TryGetValue(to, out var toRate))
            throw new ArgumentException($"Moeda de destino não suportada: {toCurrency}.", nameof(toCurrency));

        var converted = amount * fromRate / toRate;
        return Math.Round(converted, 2, MidpointRounding.AwayFromZero);
    }

    private static string Normalize(string currency) =>
        currency.Trim().ToUpperInvariant();
}
