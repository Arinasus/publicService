using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

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
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return await _context.Notifications
                .Include(n => n.User)
                .ToListAsync();
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NotificationID == id);

            if (notification == null)
            {
                return NotFound();
            }

            return notification;
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = notification.NotificationID }, notification);
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, Notification notification)
        {
            if (id != notification.NotificationID)
            {
                return BadRequest();
            }

            _context.Entry(notification).State = EntityState.Modified;
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
