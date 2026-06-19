using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.Users;

public class UserRegisterDto
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Mínimo 8 caracteres")]
    public string Password { get; set; } = "";

    public string ProfilePhoto { get; set; } = "";
}
