using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.Accounts;

public class AccountCreateDto
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    public decimal InitialBalance { get; set; }

    public int? UserId { get; set; }
}
