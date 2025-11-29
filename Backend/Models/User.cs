using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Backend.Models;
namespace Backend.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    }
}