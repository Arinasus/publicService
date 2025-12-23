namespace Backend.DTOs
{
    public class ProviderDto
    {
        public int ProviderID { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<ServiceDto> Services { get; set; } = new();
    }

    public class ServiceDto
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

}
