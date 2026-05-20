using System.ComponentModel.DataAnnotations;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Entities;

public class TransactionUpdateDto
{
    [Required]
    public int TransactionId { get; set; }

    [Required]
    [StringLength(250)]
    public string TransactionDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal TransactionValue { get; set; }

    public DateTime Date { get; set; }

    [Range(1, 2)]
    public int TransactionTypeId { get; set; }

    public int CategoryId { get; set; }

    public int AccountId { get; set; }

    public TransactionStatus Status { get; set; }
}
