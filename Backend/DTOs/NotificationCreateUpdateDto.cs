using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class NotificationCreateDto
    {
        [Required]
        public DateTime NotificationDate { get; set; }

        [Required]
        [RegularExpression("^(Info|Warning|Payment|Reminder)$", ErrorMessage = "Недопустимый тип уведомления.")]
        public string NotificationType { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string NotificationText { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        [Range(1, int.MaxValue, ErrorMessage = "Выберите пользователя.")]
        public int UserID { get; set; }
    }

    public class NotificationUpdateDto : NotificationCreateDto
    {
        [Range(1, int.MaxValue)]
        public int NotificationID { get; set; }
    }
}
