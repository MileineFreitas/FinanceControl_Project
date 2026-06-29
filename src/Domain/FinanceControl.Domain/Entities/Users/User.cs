using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations.Schema;using System.Text.Json.Serialization;
using FinanceControl.Contracts.Interfaces.Entities.Users;
using FinanceControl.Domain.Entities.Accounts;using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Entities.Transactions;
namespace FinanceControl.Domain.Entities.Users;
[Table("Users")]
public class User : IUser
{    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();
    [Required(ErrorMessage = "Nome é obrigatorio")]    [StringLength(100)]    public string? UserName { get; set; }
    [Required(ErrorMessage = "Informe seu email")]
    [StringLength(200)]
    public string? UserEmail { get; set; }
    [Required]
    [StringLength(20)]
    public string? Password { get; set; }
    public string? ProfilePhoto { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid SecurityStamp { get; set; } = Guid.NewGuid();
    public DateTime DateCreated { get; set; }
    public string Currency { get; set; } = "BRL";
    public string Language { get; set; } = "pt-BR";
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public int FinancialMonthStartDay { get; set; } = 1;
    [JsonIgnore]
    public ICollection<Transaction>? Transactions { get; set; }
    [JsonIgnore]
    public ICollection<Category>? Categories { get; set; }
    [JsonIgnore]
    public ICollection<Account>? Accounts { get; set; }
    [JsonIgnore]
    public ICollection<PaymentMethod>? PaymentMethods { get; set; }
}