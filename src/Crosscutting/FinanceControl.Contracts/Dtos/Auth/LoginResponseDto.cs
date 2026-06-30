using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Contracts.Dtos.Auth;

public class LoginResponseDto
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Guid SecurityStamp { get; set; }

    public string Moeda { get; set; } = FinancialPreferenceDefaults.Moeda;

    public string Idioma { get; set; } = FinancialPreferenceDefaults.Idioma;

    public string FormatoData { get; set; } = FinancialPreferenceDefaults.FormatoData;

    public int InicioMes { get; set; } = FinancialPreferenceDefaults.InicioMes;
}
