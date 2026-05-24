using FinanceControl.Contracts.Interfaces.Entities.TransactionTypes;

namespace FinanceControl.Contracts.Dtos.TransactionTypes;

public class TransactionTypeDto : ITransactionType
{
    public Guid TransactionTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = "💳";

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }
}
