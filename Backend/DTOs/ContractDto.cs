using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class ContractDto
    {
        public int ContractID { get; set; }

        [Required(ErrorMessage = "Номер договора обязателен")]
        [StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Дата начала договора обязательна")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(ContractDto), nameof(ValidateStartDate))]
        public DateTime ContractStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ContractEndDate { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int AddressID { get; set; }
        public string Address { get; set; } = string.Empty; 
        public int ProviderID { get; set; }
        public string ProviderName { get; set; } = string.Empty;

        public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();

        public static ValidationResult? ValidateStartDate(DateTime startDate, ValidationContext context)
        {
            if (startDate < DateTime.Today)
            {
                return new ValidationResult("Дата начала договора не может быть в прошлом.");
            }
            return ValidationResult.Success;
        }
    }
}