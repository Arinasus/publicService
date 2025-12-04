using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class NotificationHistory
    {
        public int Id { get; set; }
        public int NotificationID { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }

        public Notification Notification { get; set; } = null!;
    }
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime NotificationDate { get; set; }

        public static readonly string[] AllowedTypes = { "Info", "Warning", "Payment", "Reminder" };

        [Required, StringLength(20)]
        public string NotificationType { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string NotificationText { get; set; } = string.Empty;

        public bool IsRead { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        public User? User { get; set; }
      /*  public List<NotificationHistory> History { get; set; } = new();*/
    }
}