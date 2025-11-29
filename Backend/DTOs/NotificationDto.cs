namespace Backend.DTOs
{
    public class NotificationDto
    {
        public int NotificationID { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public string NotificationText { get; set; } = string.Empty;
        public bool IsRead { get; set; }

        public string UserEmail { get; set; } = string.Empty;
    }
}
