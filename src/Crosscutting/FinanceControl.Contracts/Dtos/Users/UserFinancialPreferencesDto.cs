namespace FinanceControl.Contracts.Dtos.Users;

public class UserFinancialPreferencesDto
{
    public string Moeda { get; set; } = FinancialPreferenceDefaults.Moeda;

    public string Idioma { get; set; } = FinancialPreferenceDefaults.Idioma;

    public string FormatoData { get; set; } = FinancialPreferenceDefaults.FormatoData;

    public int InicioMes { get; set; } = FinancialPreferenceDefaults.InicioMes;
}

public static class FinancialPreferenceDefaults
{
    public const string Moeda = "BRL";
    public const string Idioma = "pt-BR";
    public const string FormatoData = "dd/MM/yyyy";
    public const int InicioMes = 1;

    public static readonly string[] MoedasValidas = ["BRL", "USD", "EUR"];
    public static readonly string[] IdiomasValidos = ["pt-BR", "en-US", "es-ES"];
    public static readonly string[] FormatosDataValidos = ["dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd"];
    public static readonly int[] IniciosMesValidos = [1, 5, 10, 15];
}
