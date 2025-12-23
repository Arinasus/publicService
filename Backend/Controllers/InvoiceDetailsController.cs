using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public InvoiceDetailsController(UtilitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/InvoiceDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetailsDto>> GetInvoiceDetails(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Provider)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
                return NotFound();

            // 🔎 тянем показания по ContractID и периоду
            var meterReadings = await _context.MeterReadings
                .Where(m => m.ContractID == invoice.ContractID && m.ReadingDate.Month == invoice.IssueDate.Month && m.ReadingDate.Year == invoice.IssueDate.Year)
                .ToListAsync();

            var dto = new InvoiceDetailsDto
            {
                InvoiceID = invoice.InvoiceID,
                ContractID = invoice.ContractID,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                Period = invoice.Period,
                TotalAmount = invoice.TotalAmount,
                IsPaid = invoice.IsPaid,

                Provider = new ProviderDto
                {
                    ProviderID = invoice.Contract.Provider.ProviderID,
                    ProviderName = invoice.Contract.Provider.ProviderName,
                    ContactPerson = invoice.Contract.Provider.ContactPerson,
                    PhoneNumber = invoice.Contract.Provider.PhoneNumber,
                    Email = invoice.Contract.Provider.Email,
                    IBAN = invoice.Contract.Provider.IBAN,
                    BIC = invoice.Contract.Provider.BIC,
                    UNP = invoice.Contract.Provider.UNP,
                    Services = invoice.Contract.Provider.Services.Select(s => new ServiceDto
                    {
                        ServiceID = s.ServiceID,
                        ServiceName = s.ServiceName,
                        UnitOfMeasure = s.UnitOfMeasure,
                        Price = s.Price
                    }).ToList()
                },

                Payer = new PayerDto
                {
                    Name = $"{invoice.Contract.User.FirstName} {invoice.Contract.User.LastName}",
                    Address = invoice.Contract.User.UserAddresses
        .Where(ua => ua.IsPrimary)
        .Select(ua => ua.Address != null ? ua.Address.FullAddress : "")
        .FirstOrDefault() ?? "Адрес не указан"
                },
                Meters = meterReadings.Select(m => new MeterReadingDto
                {
                    Name = "Показание", // можно расширить модель, если нужно хранить тип счётчика
                    Previous = 0,       // если храним только текущее значение, то предыдущие считаем отдельно
                    Current = m.ReadingValue
                }).ToList()
            };

            return dto;
        }
    }
}
