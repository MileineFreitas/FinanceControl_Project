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
    public Guid AccountId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal InitialBalance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<Transaction>? Transactions { get; set; }
}
