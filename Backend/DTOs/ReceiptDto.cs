using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs
{
    public class ReceiptDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
    }
}
