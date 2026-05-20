using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.Users;

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

    /// <summary>Receita ou despesa — escolhido no cadastro da transação.</summary>
    public TransactionTypeKind TransactionTypeKind { get; set; }

    /// <summary>Meio de pagamento (opcional), definido no cadastro da transação.</summary>
    public PaymentKind? PaymentKind { get; set; }

    public int CategoryId { get; set; }

    /// <summary>Incluído no JSON da API para telas (dashboard) exibirem o nome da categoria.</summary>
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
