using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class Service
    {
        [Key]
        public int ServiceID { get; set; }

        [Required, StringLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;
        [Range(0, 100000)]
        public decimal Price { get; set; }
        [Required]
        public int ProviderID { get; set; }
        public Provider? Provider { get; set; }
    }
}