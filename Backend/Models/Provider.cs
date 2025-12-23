using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Backend.Models
{
    public class Provider
    {
        [Key]
        public int ProviderID { get; set; }

        [Required, StringLength(100)]
        public string ProviderName { get; set; } = string.Empty;

        [StringLength(50)]
        public string ContactPerson { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}