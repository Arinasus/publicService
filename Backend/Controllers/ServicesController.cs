using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Services;
namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase 
    {
        private readonly UtilitiesDbContext _context;

        public ServicesController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/Services
[HttpGet]
public async Task<ActionResult<IEnumerable<object>>> GetServices()
{
    try
    {
        return await _context.Services
            .Include(s => s.Provider)
            .Select(s => new
            {
                s.ServiceID,
                s.ServiceName,
                s.UnitOfMeasure,
                s.Price,
                s.ProviderID,
                ProviderName = s.Provider != null ? s.Provider.ProviderName ?? "" : "",
                s.ContractID
            })
            .ToListAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetServices: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

// GET: api/Services/5
[HttpGet("{id}")]
public async Task<ActionResult<Service>> GetService(int id)
{
    var service = await _context.Services
        .Include(s => s.Provider)
        .FirstOrDefaultAsync(s => s.ServiceID == id);

    if (service == null)
    {
        return NotFound();
    }

    return service;
}

[HttpGet("me")]
public async Task<IActionResult> GetMyServices()
{
    try
    {
        // Используем AuthHelpers напрямую
        var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
        if (user == null) return Unauthorized("User not found");

        var userId = user.UserID;
        
        var userServices = await _context.Contracts
            .Where(c => c.UserID == userId)
            .SelectMany(c => c.Services)
            .Include(s => s.Provider)
            .Distinct()
            .Select(s => new {
                s.ServiceID,
                s.ServiceName,
                s.UnitOfMeasure,
                s.Price,
                s.ProviderID,
                ProviderName = s.Provider != null ? s.Provider.ProviderName ?? "" : ""
            })
            .ToListAsync();

        return Ok(userServices);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetMyServices: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

[HttpGet("available")]
        public async Task<IActionResult> GetAvailableServices()
        {
            try
            {
                // Используем AuthHelpers напрямую
                var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
                if (user == null) return Unauthorized("User not found");

                var userId = user.UserID;
                
                // Получаем ID услуг, которые уже есть у пользователя
                var userServiceIds = await _context.Contracts
                    .Where(c => c.UserID == userId)
                    .SelectMany(c => c.Services.Select(s => s.ServiceID))
                    .Distinct()
                    .ToListAsync();

                var availableServices = await _context.Services
                    .Include(s => s.Provider)
                    .Where(s => !userServiceIds.Contains(s.ServiceID))
                    .Select(s => new 
                    {
                        s.ServiceID,
                        s.ServiceName,
                        s.UnitOfMeasure,
                        s.Price,
                        s.ProviderID,
                        ProviderName = s.Provider != null ? s.Provider.ProviderName ?? "" : "",
                        IsAvailable = true
                    })
                    .ToListAsync();

                return Ok(availableServices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableServices: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // POST: api/Services
        [HttpPost]
        public async Task<ActionResult<Service>> PostService(Service service)
        {
            try
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                // Возвращаем созданный объект
                return CreatedAtAction(nameof(GetService), new { id = service.ServiceID }, service);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostService: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, Service service)
        {
            if (id != service.ServiceID)
            {
                return BadRequest("Service ID mismatch");
            }

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PutService: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    return NotFound();
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteService: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}

