namespace Backend.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }

        public string ContractNumber { get; set; } = string.Empty;
    }
}
