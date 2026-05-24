using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Interfaces.Entities.Transactions;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.Transactions;

[Table("Transactions")]
public class Transaction : ITransaction
{
    [Key]
    public Guid TransactionId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(250)]
    public string TransactionDescription { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TransactionValue { get; set; }

    public DateTimeOffset Date { get; set; }

    public TransactionTypeKind TransactionTypeKind { get; set; }

    public Guid PaymentMethodId { get; set; }

    [JsonIgnore]
    public PaymentMethod? PaymentMethod { get; set; }

    public Guid CategoryId { get; set; }

    public Category? Category { get; set; }

    public Guid AccountId { get; set; }

    [JsonIgnore]
    public Account? Account { get; set; }

    public Guid UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    public TransactionStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
