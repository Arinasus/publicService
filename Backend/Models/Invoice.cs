using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class Invoice
{
    [Key]
    public int InvoiceID { get; set; }

    [Required]
    public int ContractID { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime InvoiceDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    [DataType(DataType.Date)]
    public DateTime? PaymentDate { get; set; }

    public Contract? Contract { get; set; }
}
}