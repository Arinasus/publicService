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
    public class InvoicesController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public InvoicesController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Contract)
                .Select(i => new InvoiceDto
                {
                    InvoiceID = i.InvoiceID,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    TotalAmount = i.TotalAmount,
                    IsPaid = i.IsPaid,
                    ContractNumber = i.Contract.ContractNumber
                })
                .ToListAsync();

            return Ok(invoices);
        }
        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                .Where(i => i.InvoiceID == id)
                .Select(i => new InvoiceDto
                {
                    InvoiceID = i.InvoiceID,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    TotalAmount = i.TotalAmount,
                    IsPaid = i.IsPaid,
                    ContractNumber = i.Contract.ContractNumber
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetPaymentHistoryByUser(int userId)
        {
            var invoices = await _context.Invoices
                .Include(i => i.Contract)
                .ThenInclude(c => c.User)
                .Where(i => i.IsPaid && i.Contract.UserID == userId)
                .Select(i => new
                {
                    i.InvoiceID,
                    i.TotalAmount,
                    i.PaymentDate,
                    ContractNumber = i.Contract.ContractNumber,
                    UserEmail = i.Contract.User.Email
                })
                .ToListAsync();

            if (!invoices.Any())
                return NotFound();

            return Ok(invoices);
        }



        // POST: api/Invoices/{id}/pay
        /*[HttpPost("{id}/pay")]
        public async Task<IActionResult> PayInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            if (invoice.IsPaid)
            {
                return BadRequest("Invoice already paid");
            }

            invoice.IsPaid = true;
            invoice.PaymentDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Invoice paid successfully" });
        }*/
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayInvoice(int id, [FromBody] PaymentDto dto)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                .ThenInclude(c => c.User) // если у тебя есть связь с пользователем
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
                return NotFound();

            if (invoice.IsPaid)
                return BadRequest("Invoice already paid");

            invoice.IsPaid = true;
            invoice.PaymentDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await _context.SaveChangesAsync();

            // Формируем DTO для чека
            var receipt = new ReceiptDto
            {
                StoreName = "Utilities Service",
                Date = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm"),
                Customer = invoice.Contract?.User != null  ? invoice.Contract.User.FirstName  : "Unknown",
                InvoiceNumber = invoice.InvoiceID,
                Amount = invoice.TotalAmount,
                Method = dto.PaymentMethod
            };

            var pdfBytes = ReceiptPdfGenerator.Generate(receipt);

            return File(pdfBytes, "application/pdf", $"receipt_{invoice.InvoiceID}.pdf");
        }


        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> PostInvoice(InvoiceDto dto)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractNumber == dto.ContractNumber);

            if (contract == null)
            {
                return BadRequest("Contract not found");
            }

            var invoice = new Invoice
            {
                InvoiceDate = DateTime.SpecifyKind(dto.InvoiceDate, DateTimeKind.Utc),
                DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc),
                TotalAmount = dto.TotalAmount,
                IsPaid = dto.IsPaid,
                ContractID = contract.ContractID,
                PaymentDate = dto.IsPaid ? DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) : null
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            dto.InvoiceID = invoice.InvoiceID;
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceID }, dto);
        }


        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, InvoiceDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.InvoiceDate = DateTime.SpecifyKind(dto.InvoiceDate, DateTimeKind.Utc);
            invoice.DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);
            invoice.TotalAmount = dto.TotalAmount;
            invoice.IsPaid = dto.IsPaid;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        
    }
}
