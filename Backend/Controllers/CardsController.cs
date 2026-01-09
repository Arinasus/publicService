using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using System.Security.Claims;
using Backend.Services;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;
        public CardsController(UtilitiesDbContext context) { _context = context; }

        // GET: api/Cards/me
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<CardDTO>>> GetMyCards()
        {
            var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
            if (user == null) return Unauthorized("Нет или неверный токен");

            var cards = await _context.Cards
                .Where(c => c.UserID == user.UserID)
                .ToListAsync();

            var result = cards.Select(c => new CardDTO
            {
                CardID = c.CardID,
                UserID = c.UserID,
                CardNumber = MaskCardNumber(c.CardNumber),
                ExpiryDate = c.ExpiryDate,
                CardHolderName = c.CardHolderName
            }).ToList();

            return Ok(result);
        }


        // GET: api/Cards/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CardDTO>>> GetCardsByUser(int userId)
        {
            var cards = await _context.Cards
                .Where(c => c.UserID == userId)
                .ToListAsync();

            var result = cards.Select(c => new CardDTO
            {
                CardID = c.CardID,
                UserID = c.UserID,
                CardNumber = MaskCardNumber(c.CardNumber),
                ExpiryDate = c.ExpiryDate,
                CardHolderName = c.CardHolderName
            }).ToList();

            return Ok(result);
        }

        private static string MaskCardNumber(string? number)
        {
            if (string.IsNullOrEmpty(number) || number.Length < 4)
                return "**** **** **** ****";

            return "**** **** **** " + number.Substring(number.Length - 4);
        }

        // POST: api/Cards
        [HttpPost]
        public async Task<ActionResult<CardDTO>> PostCard(CardDTO dto)
        {
            var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
            if (user == null) return Unauthorized("Нет или неверный токен");

            var card = new Card
            {
                UserID = user.UserID,
                CardNumber = dto.CardNumber,
                ExpiryDate = dto.ExpiryDate,
                CardHolderName = dto.CardHolderName,
                CVV = "000"
            };

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            dto.CardID = card.CardID;
            dto.UserID = user.UserID;
            dto.CardNumber = MaskCardNumber(dto.CardNumber);

            return CreatedAtAction(nameof(GetMyCards), new { }, dto);
        }


        // PUT: api/Cards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(int id, CardDTO dto)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound();

            card.CardNumber = dto.CardNumber;
            card.ExpiryDate = dto.ExpiryDate;
            card.CardHolderName = dto.CardHolderName;

            _context.Entry(card).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Cards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound();

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
