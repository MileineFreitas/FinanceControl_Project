using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.Accounts;

public class AccountUpdateDto
{
    public int AccountId { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    public decimal InitialBalance { get; set; }

    public decimal CurrentBalance { get; set; }
}
