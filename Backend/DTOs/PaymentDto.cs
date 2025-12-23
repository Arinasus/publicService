namespace Backend.DTOs
{
    public class PaymentDto
    {
        public int PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public int InvoiceID { get; set; }
        public string ContractNumber { get; set; } = string.Empty; 
        public string CardNumber { get; set; } = string.Empty;
    }
}
