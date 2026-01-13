using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs
{
    public class ReceiptDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;

        public string OperationId { get; set; } = Guid.NewGuid().ToString(); // уникальный номер операции 
        public string ProviderName { get; set; } = string.Empty; 
        public string ProviderUNP { get; set; } = string.Empty;
        public string ProviderIBAN { get; set; } = string.Empty; 
        
        public List<MeterReadingDto> Meters { get; set; } = new();
    }
}
