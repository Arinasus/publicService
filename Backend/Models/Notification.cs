using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime NotificationDate { get; set; }

        [Required, StringLength(20)]
        public string NotificationType { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string NotificationText { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public User? User { get; set; }
    }
}