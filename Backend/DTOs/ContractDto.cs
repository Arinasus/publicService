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

        public string UserName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;

        // Кастомная проверка
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
