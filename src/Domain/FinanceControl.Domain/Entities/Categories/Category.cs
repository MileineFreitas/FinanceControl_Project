using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Domain.Entities.TransactionTypes;
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

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    /// <summary>
    /// Opcional no modelo de negócio: categorias podem ser só de receita ou só de despesa.
    /// </summary>
    public int? TransactionTypeId { get; set; }

    [JsonIgnore]
    public TransactionTypeDefinition? TransactionTypeDefinition { get; set; }

    [JsonIgnore]
    public ICollection<Transaction>? Transactions { get; set; }
}
