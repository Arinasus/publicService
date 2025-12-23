using Backend.Data;
using Backend.Models;
using Backend.Services;
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
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await AuthHelpers.GetUserByBearerTokenAsync(_db, Request);
            if (user == null) return Unauthorized("Нет или неверный токен");


            var u = await _db.Users
                .Include(x => x.UserAddresses)
                .ThenInclude(ua => ua.Address) // подтягиваем реальные адреса
                .FirstOrDefaultAsync(x => x.UserID == user.UserID);
            if (u == null) return NotFound();

            return Ok(new
            {
                userID = u.UserID,
                firstName = u.FirstName,
                lastName = u.LastName,
                email = u.Email,
                addresses = u.UserAddresses.Select(ua => new {
                    addressID = ua.AddressID,
                    addressLine = $"{ua.Address?.City}, {ua.Address?.Street}, {ua.Address?.PostalCode}",
                    isPrimary = ua.IsPrimary
                })
            });
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
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] RoleDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = dto.Role == "admin" ? "admin" : "user";
            await _db.SaveChangesAsync();

            return Ok(new { message = "Роль изменена", role = user.Role });
        }


        public class RoleDto
        {
            public string Role { get; set; } = "user";
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
