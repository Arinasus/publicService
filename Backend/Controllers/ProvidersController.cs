using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public ProvidersController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/Providers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
        {
            return await _context.Providers.ToListAsync();
        }

        // GET: api/Providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetProvider(int id)
        {
            var provider = await _context.Providers.FindAsync(id);

            if (provider == null)
            {
                return NotFound();
            }

            return provider;
        }

        // POST: api/Providers
        [HttpPost]
        public async Task<ActionResult<Provider>> PostProvider(Provider provider)
        {
            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProvider), new { id = provider.ProviderID }, provider);
        }

        // PUT: api/Providers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProvider(int id, Provider provider)
        {
            if (id != provider.ProviderID)
            {
                return BadRequest();
            }

            _context.Entry(provider).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Providers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider(int id)
        {
            var provider = await _context.Providers.FindAsync(id);
            if (provider == null)
            {
                return NotFound();
            }

            _context.Providers.Remove(provider);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
