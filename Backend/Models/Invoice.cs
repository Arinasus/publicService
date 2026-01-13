using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class Invoice
{
    [Key]
    public int InvoiceID { get; set; }

    [Required]
    public int ContractID { get; set; }
    // дата выставления счёта
    [Required, DataType(DataType.Date)]
    public DateTime IssueDate { get; set; }
    // срок оплаты (крайний день)
    [Required, DataType(DataType.Date)]
    public DateTime DueDate { get; set; }
        // период, за который выставлен счёт
    [Required, StringLength(7)] 
    public string Period { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    [Required]
    public bool IsPaid { get; set; } = false;

    [DataType(DataType.Date)]
    public DateTime? PaymentDate { get; set; }
    public decimal Penalty { get; set; } = 0;
    public Contract? Contract { get; set; }
}
}