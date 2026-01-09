using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

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
        public async Task<ActionResult<IEnumerable<ProviderDto>>> GetProviders() 
        { 
            var providers = await _context.Providers
                .Include(p => p.Services)
                .Select(p => new ProviderDto { 
                    ProviderID = p.ProviderID, 
                    ProviderName = p.ProviderName, 
                    ContactPerson = p.ContactPerson, 
                    PhoneNumber = p.PhoneNumber, 
                    Email = p.Email, 
                    IBAN = p.IBAN, 
                    BIC = p.BIC, 
                    UNP = p.UNP, 
                    Services = p.Services.Select(s => new ServiceDto { 
                        ServiceID = s.ServiceID, 
                        ServiceName = s.ServiceName, 
                        UnitOfMeasure = s.UnitOfMeasure, 
                        Price = s.Price })
                    .ToList() })
                .ToListAsync(); 
            return providers; 
        }

        // GET: api/Providers/5
        [HttpGet("{id}")] public async Task<ActionResult<ProviderDto>> GetProvider(int id) { 
            var provider = await _context.Providers
                .Include(p => p.Services)
                .Where(p => p.ProviderID == id)
                .Select(p => new ProviderDto 
                { 
                    ProviderID = p.ProviderID, 
                    ProviderName = p.ProviderName, 
                    ContactPerson = p.ContactPerson, 
                    PhoneNumber = p.PhoneNumber, 
                    Email = p.Email, 
                    IBAN = p.IBAN, BIC = p.BIC, UNP = p.UNP,
                    Services = p.Services
                    .Select(s => new ServiceDto 
                    { 
                        ServiceID = s.ServiceID, 
                        ServiceName = s.ServiceName, 
                        UnitOfMeasure = s.UnitOfMeasure, Price = s.Price })
                    .ToList() 
                })
                .FirstOrDefaultAsync(); 
            if (provider == null) { 
                return NotFound();
            } return provider; 
        }

        // POST: api/Providers
        [HttpPost] public async Task<ActionResult<ProviderDto>> PostProvider(Provider provider) 
        { 
            _context.Providers.Add(provider); 
            await _context.SaveChangesAsync(); 
            return CreatedAtAction(nameof(GetProvider), 
                new { id = provider.ProviderID }, 
                new ProviderDto { ProviderID = provider.ProviderID, 
                    ProviderName = provider.ProviderName, 
                    ContactPerson = provider.ContactPerson, 
                    PhoneNumber = provider.PhoneNumber, 
                    Email = provider.Email, 
                    IBAN = provider.IBAN, 
                    BIC = provider.BIC, 
                    UNP = provider.UNP, 
                    Services = new List<ServiceDto>() 
                });
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
