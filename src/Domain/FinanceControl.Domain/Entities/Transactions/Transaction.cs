using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Entities.Transactions;

[Table("Transactions")]
public class Transaction
{
    [Key]
    public int TransactionId { get; set; }

    [Required]
    [StringLength(250)]
    public string TransactionDescription { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TransactionValue { get; set; }

    public DateTime Date { get; set; }

    public int TransactionTypeId { get; set; }

    [JsonIgnore]
    public TransactionTypeDefinition? TransactionTypeDefinition { get; set; }

    public int CategoryId { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }

    public int AccountId { get; set; }

    [JsonIgnore]
    public Account? Account { get; set; }

    public int UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    public TransactionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
