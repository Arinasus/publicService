using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int InvoiceID { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PaymentAmount { get; set; }

        [Required, StringLength(30)]
        public string PaymentMethod { get; set; } = string.Empty;

        public Invoice? Invoice { get; set; }
    }
}