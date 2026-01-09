using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs
{
    public class ServiceDto
    {
        public int ServiceID { get; set; } public 
        string ServiceName { get; set; } = string.Empty; 
        public string UnitOfMeasure { get; set; } = string.Empty; 
        public decimal Price { get; set; } 
        public int? ProviderID { get; set; } 
        public string? ProviderName { get; set; }
    }
    // public class ServiceCreateDto { 
    //     public string ServiceName { get; set; } = string.Empty; 
    //     public string UnitOfMeasure { get; set; } = string.Empty; 
    //     public decimal Price { get; set; } 
    //     public int ProviderID { get; set; } 
    //     }
}