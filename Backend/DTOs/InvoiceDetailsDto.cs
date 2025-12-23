namespace Backend.DTOs
{
    public class InvoiceDetailsDto
    {
        public int InvoiceID { get; set; }
        public int ContractID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }

        public ProviderDto Provider { get; set; } = new();
        public PayerDto Payer { get; set; } = new();
        public List<MeterReadingDto> Meters { get; set; } = new();
    }

    public class PayerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class MeterReadingDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Previous { get; set; }
        public decimal Current { get; set; }
    }

}
