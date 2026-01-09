namespace Backend.DTOs
{
    public class ProviderCreateDto
    {
        public string ProviderName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BIC { get; set; } = string.Empty;
        public string UNP { get; set; } = string.Empty;
    }

}
