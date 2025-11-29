using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class UserAddress
    {
        [Key]
        public int UserAddressID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int AddressID { get; set; }

        public bool IsPrimary { get; set; }

        public User? User { get; set; }
        public Address? Address { get; set; }
    }
}