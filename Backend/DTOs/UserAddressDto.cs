namespace Backend.DTOs 
{ 
    public class UserAddressDto 
    { 
        public int UserAddressID { get; set; } 
        public int UserID { get; set; } 
        public int AddressID { get; set; } 
        public bool IsPrimary { get; set; } 
        public string? Street { get; set; } 
        public string? City { get; set; } 
        public string? House { get; set; } 
        public string? Apartment { get; set; } 
        public string? PostalCode { get; set; } 
    } 
}