using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Entities.Categories;

[Table("Categories")]
public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Nome da categoria é obrigatorio...")]
    [StringLength(40)]
    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public int? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public TransactionType Type { get; set; }
}
