using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Card
    {
        [Key]
        public int CardID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        [Required, StringLength(16)]
        public string CardNumber { get; set; } = string.Empty;

        [Required, StringLength(5)]
        public string ExpiryDate { get; set; } = string.Empty; // формат MM/YY

        [Required, StringLength(3)]
        public string CVV { get; set; } = string.Empty;

        public string CardHolderName { get; set; } = string.Empty;
    }

}
