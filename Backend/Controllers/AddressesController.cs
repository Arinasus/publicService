using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public AddressesController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses
                .Include(a => a.UserAddresses)
                    .ThenInclude(ua => ua.User)
                .ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses
                .Include(a => a.UserAddresses)
                    .ThenInclude(ua => ua.User)
                .FirstOrDefaultAsync(a => a.AddressID == id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAddress),
                new { id = address.AddressID },
                address
            );
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressID) return BadRequest();

            _context.Entry(address).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
