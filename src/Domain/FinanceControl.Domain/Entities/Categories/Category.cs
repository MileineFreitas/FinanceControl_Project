using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.Categories;

[Table("Categories")]
public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Nome da categoria é obrigatorio...")]
    [StringLength(40)]
    public string? CategoryName { get; set; }

    public int? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public Transaction? Transaction { get; set; }
}
