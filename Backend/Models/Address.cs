using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Backend.Models;
namespace Backend.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required, StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    }
}