using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAddressesController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public UserAddressesController(UtilitiesDbContext context)
        {
            _context = context;
        }
        // GET: api/UserAddress/user/5 
        [HttpGet("user/{userId}")] 
        public async Task<ActionResult<IEnumerable<UserAddressDto>>> GetUserAddresses(int userId) 
        { 
            var query = from ua in _context.UserAddresses join a in _context.Addresses on ua.AddressID equals a.AddressID where ua.UserID == userId select new UserAddressDto { 
                UserAddressID = ua.UserAddressID, 
                UserID = ua.UserID, 
                AddressID = ua.AddressID, 
                IsPrimary = ua.IsPrimary, 
            Street = a.Street, 
            City = a.City, 
            House = a.House, 
            Apartment = a.Apartment, 
            PostalCode = a.PostalCode 
            }; 
            return Ok(await query.ToListAsync()); 
        }
    
        // POST: api/UserAddresses
        [HttpPost]
        public async Task<IActionResult> CreateUserAddress([FromBody] UserAddressCreateDto dto)
        {
            var userExists = await _context.Users.FindAsync(dto.UserID);
            var addressExists = await _context.Addresses.FindAsync(dto.AddressID);

            if (userExists == null || addressExists == null)
                return BadRequest("User or Address not found");

            var userAddress = new UserAddress
            {
                UserID = dto.UserID,
                AddressID = dto.AddressID,
                IsPrimary = dto.IsPrimary
            };

            _context.UserAddresses.Add(userAddress);
            await _context.SaveChangesAsync();

            return Ok(userAddress);
        }
    }
}
