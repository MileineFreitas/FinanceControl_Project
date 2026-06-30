using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.Auth;

public class DeleteAccountRequestDto
{
    [Required(ErrorMessage = "Informe sua senha para confirmar a exclusão.")]
    public string Password { get; set; } = string.Empty;
}
