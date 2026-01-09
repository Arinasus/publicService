namespace Backend.DTOs
{
    public class UserAddressCreateDto
    {
        public int UserID { get; set; }
        public int AddressID { get; set; }
        public bool IsPrimary { get; set; }
    }
}
