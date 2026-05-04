using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.Accounts;

[Table("Accounts")]
public class Account
{
    [Key]
    public int AccountId { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal InitialBalance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<Transaction>? Transactions { get; set; }
}
