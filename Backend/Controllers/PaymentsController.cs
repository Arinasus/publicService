using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public PaymentsController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i.Contract)
                .Include(p => p.Card)
                .ToListAsync();
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.PaymentID == id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByUser(int userId)
        {
            var payments = await _context.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Contract)
                .Include(p => p.Card)
                .Where(p => p.Invoice.Contract.UserID == userId)
                .Select(p => new PaymentDto
                {
                    PaymentID = p.PaymentID,
                    InvoiceID = p.InvoiceID,
                    PaymentAmount = p.PaymentAmount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = (DateTime)p.PaymentDate,
                    ContractNumber = p.Invoice.Contract.ContractNumber,
                    CardNumber = p.Card != null ? p.Card.CardNumber : null
                })
                .ToListAsync();

            return Ok(payments);
        }
        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                .FirstOrDefaultAsync(i => i.InvoiceID == payment.InvoiceID);

            if (invoice == null) return BadRequest("Invoice not found");
            if (invoice.IsPaid) return BadRequest("Invoice already paid");

            if (payment.CardID.HasValue)
            {
                var card = await _context.Cards
                    .FirstOrDefaultAsync(c => c.CardID == payment.CardID.Value && c.UserID == invoice.Contract.UserID);

                if (card == null) return BadRequest("Card not found or does not belong to user");
            }

            // имитация списания средств
            invoice.IsPaid = true;
            invoice.PaymentDate = DateTime.UtcNow;

            payment.PaymentDate = invoice.PaymentDate;
            payment.PaymentAmount = invoice.TotalAmount;
            payment.PaymentMethod = payment.CardID.HasValue ? "Card" : "Cash";

            _context.Payments.Add(payment);
            _context.Entry(invoice).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentID }, payment);
        }



        // PUT: api/Payments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.PaymentID)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
