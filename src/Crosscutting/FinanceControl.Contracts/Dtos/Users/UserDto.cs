namespace FinanceControl.Contracts.Dtos.Users;

public class UserDto
{
    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserEmail { get; set; }

    public string? ProfilePhoto { get; set; }

    public bool IsActive { get; set; }

    public string Moeda { get; set; } = FinancialPreferenceDefaults.Moeda;

    public string Idioma { get; set; } = FinancialPreferenceDefaults.Idioma;

    public string FormatoData { get; set; } = FinancialPreferenceDefaults.FormatoData;

    public int InicioMes { get; set; } = FinancialPreferenceDefaults.InicioMes;
}
