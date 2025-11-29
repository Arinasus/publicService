using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class Contract
    {
        [Key]
        public int ContractID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int AddressID { get; set; }

        [Required]
        public int ServiceID { get; set; }

        [Required]
        public int ProviderID { get; set; }

        [Required, StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        [CustomValidation(typeof(Contract), nameof(ValidateStartDate))]
        public DateTime ContractStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ContractEndDate { get; set; }

        public User? User { get; set; }
        public Address? Address { get; set; }
        public Service? Service { get; set; }
        public Provider? Provider { get; set; }

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