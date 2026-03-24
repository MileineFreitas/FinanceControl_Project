using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceControl.Domain.Entities;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [MinLength(3, ErrorMessage = "Mínimo 3 caracteres")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Mínimo 8 caracteres")]
    public string Password { get; set; } = "";

    public string PhtoBase64 { get; set; } = "";
}
