using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class AuthToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserID { get; set; }

        public User User { get; set; } = null!;
    }
}
