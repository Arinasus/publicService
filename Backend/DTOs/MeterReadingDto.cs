namespace Backend.DTOs
{
    public class MeterReadingDto
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty; 
        public decimal PreviousValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
