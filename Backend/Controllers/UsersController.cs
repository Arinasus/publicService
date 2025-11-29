using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UtilitiesDbContext _db;

        public UsersController(UtilitiesDbContext db)
        {
            _db = db;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _db.Users.ToListAsync());
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.UserID) return BadRequest();
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
