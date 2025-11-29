using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class MeterReading
    {
        [Key]
        public int ReadingID { get; set; }

        [Required]
        public int ContractID { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime ReadingDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ReadingValue { get; set; }

        public int? SubmittedByUserID { get; set; }

        public Contract? Contract { get; set; }
        public User? SubmittedByUser { get; set; }
    }
}