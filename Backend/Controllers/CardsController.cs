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
    public class CardsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;
        public CardsController(UtilitiesDbContext context) { _context = context; } 
        // GET: api/Cards/user/5
        [HttpGet("user/{userId}")] 
        public async Task<ActionResult<IEnumerable<CardDTO>>> GetCardsByUser(int userId) 
        { 
            var cards = await _context.Cards 
                .Where(c => c.UserID == userId) 
                .Select(c => new CardDTO
                { 
                    CardID = c.CardID, 
                    UserID = c.UserID, 
                    CardNumber = c.CardNumber,
                    ExpiryDate = c.ExpiryDate, 
                    CardHolderName = c.CardHolderName }) 
                .ToListAsync(); return Ok(cards); } 
        // POST: api/Cards
        [HttpPost] 
        public async Task<ActionResult<CardDTO>> PostCard(CardDTO dto) 
        { 
            var card = new Card { 
                UserID = dto.UserID, 
                CardNumber = dto.CardNumber, 
                ExpiryDate = dto.ExpiryDate, 
                CardHolderName = dto.CardHolderName, CVV = "000" // CVV можно принимать отдельно, но не возвращать
             }; _context.Cards.Add(card); 
            await _context.SaveChangesAsync(); 
            dto.CardID = card.CardID;
            return CreatedAtAction(nameof(GetCardsByUser), new { userId = dto.UserID }, dto); 
        }
        // DELETE: api/Cards/5
        [HttpDelete("{id}")] public async Task<IActionResult> DeleteCard(int id) { 
            var card = await _context.Cards.FindAsync(id); 
            if (card == null) 
                return NotFound(); _context.Cards.Remove(card); 
            await _context.SaveChangesAsync(); 
            return NoContent(); 
        } 
    }
}
