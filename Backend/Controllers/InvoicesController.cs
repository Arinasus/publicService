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
    public class InvoicesController : BaseController
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
                    IssueDate = i.IssueDate, 
                    DueDate = i.DueDate, 
                    Period = i.Period, 
                    TotalAmount = i.TotalAmount, 
                    IsPaid = i.IsPaid, 
                    PaymentDate = i.PaymentDate, 
                    ContractNumber = i.Contract.ContractNumber })
                .ToListAsync();

            return Ok(invoices);
        }
        // GET: api/Invoices/id
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                .ThenInclude(c => c.Address)
                .Where(i => i.InvoiceID == id)
                .Select(i => new InvoiceDto
                {
                    InvoiceID = i.InvoiceID,
                    IssueDate = i.IssueDate,
                    DueDate = i.DueDate,
                    Period = i.Period,
                    TotalAmount = i.TotalAmount,
                    IsPaid = i.IsPaid,
                    PaymentDate = i.PaymentDate,
                    ContractNumber = i.Contract.ContractNumber,
                    AddressLine = $"{i.Contract.Address.City}, {i.Contract.Address.Street}, {i.Contract.Address.House}"
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
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInvoices()
        {
            var userId = GetUserIdFromToken();

            var invoices = await _context.Invoices
                .Where(i => i.Contract != null && i.Contract.User != null && i.Contract.User.UserID == userId)
                .Select(i => new {
                    i.InvoiceID,
                    i.ContractID,
                    i.IssueDate,
                    i.DueDate,
                    i.Period,
                    i.TotalAmount,
                    i.IsPaid,
                    i.PaymentDate
                })
                .ToListAsync();

            return Ok(invoices);
        }


        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayInvoice(int id, [FromBody] PaymentDto dto)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                .ThenInclude(c => c.User) 
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
                InvoiceNumber = invoice.Contract.ContractNumber,
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

            var invoice = new Invoice { 
                IssueDate = DateTime.SpecifyKind(dto.IssueDate, DateTimeKind.Utc), 
                DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc), 
                Period = dto.Period, 
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


        // PUT: api/Invoices/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, InvoiceDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IssueDate = DateTime.SpecifyKind(dto.IssueDate, DateTimeKind.Utc); 
            invoice.DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc); 
            invoice.Period = dto.Period; 
            invoice.TotalAmount = dto.TotalAmount; 
            invoice.IsPaid = dto.IsPaid; 
            invoice.PaymentDate = dto.IsPaid ? DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) : null;

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
