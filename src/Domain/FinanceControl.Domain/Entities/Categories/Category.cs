using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Interfaces.Entities.Categories;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.Categories;

[Table("Categories")]
public class Category : ICategory
{
    [Key]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Nome da categoria é obrigatorio...")]
    [StringLength(40)]
    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    /// <summary>Emoji ilustrativo da categoria.</summary>
    [StringLength(16)]
    public string Icon { get; set; } = CategoryIcons.Default;

    public DateTime DateCreated { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<Transaction>? Transactions { get; set; }
}
