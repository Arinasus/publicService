using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class NotificationDto
    {
        public int NotificationID { get; set; }
        public DateTime NotificationDate { get; set; }
        [Required]
        [RegularExpression("^(Info|Warning|Payment|Reminder)$", ErrorMessage = "Недопустимый тип уведомления.")]
        public string NotificationType { get; set; } = string.Empty;
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string NotificationText { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string UserEmail { get; set; } = string.Empty;
       /* public List<HistoryDto> History { get; set; } = new();*/
    
    }
    public class HistoryDto
    {
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}
