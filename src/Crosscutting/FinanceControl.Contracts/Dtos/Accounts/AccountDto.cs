namespace FinanceControl.Contracts.Dtos.Accounts;

public class AccountDto
{
    public Guid AccountId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal InitialBalance { get; set; }

    public decimal CurrentBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? UserId { get; set; }

    public bool IsActive { get; set; }
}
