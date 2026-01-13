namespace Backend.DTOs
{
    public class ProviderDto
    {
        public int ProviderID { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty; 
        public string BIC { get; set; } = string.Empty; 
        public string UNP { get; set; } = string.Empty;
        public List<ServiceDto> Services { get; set; } = new();
    }
}
