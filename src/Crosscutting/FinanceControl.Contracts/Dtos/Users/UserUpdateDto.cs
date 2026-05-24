using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.Users;

public class UserUpdateDto
{
    public Guid UserId { get; set; }

    [Required]
    [MinLength(3)]
    public string UserName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    public string? Password { get; set; }

    public string? ProfilePhoto { get; set; }

    public bool IsActive { get; set; } = true;
}
