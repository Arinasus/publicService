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
            var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
            if (user == null) 
                return Unauthorized("Нет или неверный токен");

            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Contract)
                        .ThenInclude(c => c.Address)
                    .Where(i => i.Contract.UserID == user.UserID)
                    .Select(i => new InvoiceDto
                    {
                        InvoiceID = i.InvoiceID,
                        ContractID = i.ContractID,
                        IssueDate = i.IssueDate,
                        DueDate = i.DueDate,
                        Period = i.Period,
                        TotalAmount = i.TotalAmount,
                        IsPaid = i.IsPaid,
                        PaymentDate = i.PaymentDate,
                        ContractNumber = i.Contract.ContractNumber,
                        AddressLine = i.Contract.Address != null 
                            ? $"{i.Contract.Address.City}, {i.Contract.Address.Street}, {i.Contract.Address.House}"
                            : ""
                    })
                    .OrderByDescending(i => i.IssueDate)
                    .ToListAsync();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Invoices/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoicesByUser(int userId)
        {
            var invoices = await _context.Invoices
                .Include(i => i.Contract)
                .Where(i => i.Contract.UserID == userId)
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
                    AddressLine = i.Contract.Address != null 
                        ? $"{i.Contract.Address.City}, {i.Contract.Address.Street}, {i.Contract.Address.House}"
                        : ""
                })
                .ToListAsync();

            return Ok(invoices);
        }
    
[HttpPost("{id}/pay")]
public async Task<IActionResult> PayInvoice(int id, [FromBody] PaymentCreateDto dto)
{
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null) return Unauthorized("Нет или неверный токен");

    var invoice = await _context.Invoices
        .Include(i => i.Contract)
        .ThenInclude(c => c.User)
        .FirstOrDefaultAsync(i => i.InvoiceID == id);

    if (invoice == null) return NotFound("Invoice not found");
    if (invoice.Contract?.UserID != user.UserID) return Forbid("This invoice does not belong to you");
    if (invoice.IsPaid) return BadRequest("Invoice already paid");

    string methodText = dto.PaymentMethod;

    if (dto.PaymentMethod == "Card" && dto.CardID.HasValue)
    {
        var card = await _context.Cards
            .FirstOrDefaultAsync(c => c.CardID == dto.CardID.Value && c.UserID == user.UserID);
        if (card == null) return BadRequest("Card not found or does not belong to user");

        var last4 = card.CardNumber?.Length >= 4 ? card.CardNumber[^4..] : "****";
        methodText = $"Card (•••• {last4})";
    }

    var payment = new Payment
    {
        InvoiceID = id,
        PaymentAmount = dto.PaymentAmount,
        PaymentMethod = dto.PaymentMethod,
        PaymentDate = DateTime.UtcNow,
        CardID = dto.CardID
    };
    _context.Payments.Add(payment);

    if (dto.Meters != null && dto.Meters.Any())
    {
        foreach (var meter in dto.Meters)
        {
            var reading = new MeterReading
            {
                ContractID = invoice.ContractID,
                ServiceID = meter.ServiceID,
                ReadingDate = DateTime.UtcNow,
                ReadingValue = meter.CurrentValue
            };
            _context.MeterReadings.Add(reading);
        }
    }

    invoice.IsPaid = true;
    invoice.PaymentDate = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return Ok(new
    {
        Message = "Оплата прошла успешно",
        InvoiceID = invoice.InvoiceID,
        PaymentID = payment.PaymentID,
        Amount = payment.PaymentAmount,
        Method = methodText,
        Date = payment.PaymentDate
    });
}


[HttpGet("{id}/receipt")]
public async Task<IActionResult> GetReceipt(int id)
{
    var invoice = await _context.Invoices
        .Include(i => i.Contract)
            .ThenInclude(c => c.User)
        .Include(i => i.Contract)
            .ThenInclude(c => c.Provider)
        .FirstOrDefaultAsync(i => i.InvoiceID == id);

    if (invoice == null) return NotFound("Invoice not found");
    if (!invoice.IsPaid || invoice.PaymentDate == null) return BadRequest("Invoice not paid");

    var minskTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Minsk");
    var paidAtMinsk = TimeZoneInfo.ConvertTimeFromUtc(invoice.PaymentDate.Value, minskTz);

    // Тарифы и услуги по контракту
    var services = await _context.Services.ToListAsync();

    // Последние показания
    var meters = new List<MeterReadingDto>();
    foreach (var s in services)
    {
        var current = await _context.MeterReadings
            .Where(r => r.ContractID == invoice.ContractID && r.ServiceID == s.ServiceID && r.ReadingDate <= invoice.PaymentDate)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        var previous = await _context.MeterReadings
            .Where(r => r.ContractID == invoice.ContractID && r.ServiceID == s.ServiceID && r.ReadingDate < (current != null ? current.ReadingDate : invoice.PaymentDate))
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        meters.Add(new MeterReadingDto
        {
            ServiceID = s.ServiceID,
            ServiceName = s.ServiceName,
            Unit = s.UnitOfMeasure,
            Price = s.Price,
            PreviousValue = previous?.ReadingValue ?? 0m,
            CurrentValue = current?.ReadingValue ?? 0m
        });
    }

    var total = meters.Sum(m => (m.CurrentValue - m.PreviousValue) * m.Price);

    // Получаем последний платёж
    var payment = await _context.Payments
        .Where(p => p.InvoiceID == id)
        .OrderByDescending(p => p.PaymentDate)
        .FirstOrDefaultAsync();

    var receipt = new ReceiptDto
    {
        StoreName = "Utilities Service",
        Date = paidAtMinsk.ToString("dd.MM.yyyy HH:mm"),
        Customer = invoice.Contract?.User != null
            ? $"{invoice.Contract.User.FirstName} {invoice.Contract.User.LastName}"
            : "Unknown",
        InvoiceNumber = invoice.InvoiceID.ToString(),
        ContractNumber = invoice.Contract?.ContractNumber ?? "",
        Amount = total,
        Method = payment?.PaymentMethod ?? "Unknown",
        ProviderName = invoice.Contract?.Provider?.ProviderName ?? "",
        ProviderUNP = invoice.Contract?.Provider?.UNP ?? "",
        ProviderIBAN = invoice.Contract?.Provider?.IBAN ?? "",
        Meters = meters
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
