using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public MeterReadingsController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/MeterReadings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeterReading>>> GetMeterReadings()
        {
            return await _context.MeterReadings
                .Include(m => m.Contract)
                .Include(m => m.SubmittedByUser)
                .ToListAsync();
        }

        // GET: api/MeterReadings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeterReading>> GetMeterReading(int id)
        {
            var meterReading = await _context.MeterReadings
                .Include(m => m.Contract)
                .Include(m => m.SubmittedByUser)
                .FirstOrDefaultAsync(m => m.ReadingID == id);

            if (meterReading == null)
            {
                return NotFound();
            }

            return meterReading;
        }

        // POST: api/MeterReadings
        [HttpPost]
        public async Task<ActionResult<MeterReading>> PostMeterReading(MeterReading meterReading)
        {
            _context.MeterReadings.Add(meterReading);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMeterReading), new { id = meterReading.ReadingID }, meterReading);
        }

        // PUT: api/MeterReadings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeterReading(int id, MeterReading meterReading)
        {
            if (id != meterReading.ReadingID)
            {
                return BadRequest();
            }

            _context.Entry(meterReading).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MeterReadings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeterReading(int id)
        {
            var meterReading = await _context.MeterReadings.FindAsync(id);
            if (meterReading == null)
            {
                return NotFound();
            }

            _context.MeterReadings.Remove(meterReading);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
