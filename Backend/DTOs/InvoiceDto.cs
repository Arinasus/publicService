namespace Backend.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceID { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public DateTime DueDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
    }
}
