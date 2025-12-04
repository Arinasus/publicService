using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public NotificationsController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Select(n => new NotificationDto
                {
                    NotificationID = n.NotificationID,
                    NotificationDate = n.NotificationDate,
                    NotificationType = n.NotificationType,
                    NotificationText = n.NotificationText,
                    IsRead = n.IsRead,
                    UserEmail = n.User != null ? n.User.Email : string.Empty,
                    LastUpdatedDate = n.LastUpdatedDate
                })
                .ToListAsync();
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NotificationID == id);

            if (notification == null)
            {
                return NotFound();
            }

            return new NotificationDto
            {
                NotificationID = notification.NotificationID,
                NotificationDate = notification.NotificationDate,
                NotificationType = notification.NotificationType,
                NotificationText = notification.NotificationText,
                IsRead = notification.IsRead,
                UserEmail = notification.User != null ? notification.User.Email : string.Empty
            };
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<ActionResult<NotificationDto>> PostNotification(Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Notification.AllowedTypes.Contains(notification.NotificationType))
            {
                return BadRequest($"Недопустимый тип уведомления. Разрешено: {string.Join(", ", Notification.AllowedTypes)}");
            }
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var dto = new NotificationDto
            {
                NotificationID = notification.NotificationID,
                NotificationDate = notification.NotificationDate,
                NotificationType = notification.NotificationType,
                NotificationText = notification.NotificationText,
                IsRead = notification.IsRead,
                UserEmail = notification.User != null ? notification.User.Email : string.Empty,
                LastUpdatedDate = notification.LastUpdatedDate
            };

            return CreatedAtAction(nameof(GetNotification), new { id = notification.NotificationID }, dto);
        }

        // PUT: api/Notifications/5
        // Полный PUT для админа
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, Notification notification)
        {
            if (id != notification.NotificationID)
                return BadRequest();

            var existing = await _context.Notifications.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NotificationDate = notification.NotificationDate;
            existing.NotificationType = notification.NotificationType;
            existing.NotificationText = notification.NotificationText;
            existing.IsRead = notification.IsRead;
            existing.UserID = notification.UserID;
            existing.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Частичный PUT только для отметки прочитанным
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var existing = await _context.Notifications.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.IsRead = true;
            existing.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
