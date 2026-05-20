namespace FinanceControl.Contracts.Dtos.Accounts;

public class AccountDto
{
    public int AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal InitialBalance { get; set; }

    public decimal CurrentBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserId { get; set; }
}
