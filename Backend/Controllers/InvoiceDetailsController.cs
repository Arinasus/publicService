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
                        .ThenInclude(u => u.UserAddresses)
                            .ThenInclude(ua => ua.Address)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Services)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);

            if (invoice == null)
                return NotFound();

            // ищем последнее показание до даты выставления счета
            var previousReadings = await _context.MeterReadings .Where(m => m.ContractID == invoice.ContractID && m.ReadingDate < invoice.IssueDate) .Include(m => m.Service)
             .GroupBy(m => m.ServiceID) 
             .Select(g => g.OrderByDescending(r => r.ReadingDate).FirstOrDefault()) 
             .ToListAsync();

            var dto = new InvoiceDetailsDto
            {
                InvoiceID = invoice.InvoiceID,
                ContractID = invoice.ContractID,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                Period = $"{invoice.IssueDate:yyyy-MM}", // нормализуем период
                TotalAmount = invoice.TotalAmount,
                IsPaid = invoice.IsPaid,

                Provider = invoice.Contract?.Provider != null
                    ? new ProviderDto
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
                    }
                    : new ProviderDto
                    {
                        ProviderID = 0,
                        ProviderName = "Поставщик не указан",
                        ContactPerson = "",
                        PhoneNumber = "",
                        Email = "",
                        IBAN = "",
                        BIC = "",
                        UNP = "",
                        Services = new List<ServiceDto>()
                    },

                Payer = new PayerDto
                {
                    Name = $"{invoice.Contract.User.FirstName} {invoice.Contract.User.LastName}",
                    PayerNumber = invoice.Contract.ContractNumber,
                    Address = invoice.Contract.User.UserAddresses
                        .Where(ua => ua.IsPrimary)
                        .Select(ua => ua.Address != null ? ua.Address.FullAddress : "")
                        .FirstOrDefault() ?? "Адрес не указан"
                },

                Meters = previousReadings.Select(m => new MeterReadingDto
{
    ServiceName = m?.Service?.ServiceName ?? "Показание",
    PreviousValue = m?.ReadingValue ?? 0,
    CurrentValue = 0,
    Unit = m?.Service?.UnitOfMeasure ?? "",
    Price = m?.Service?.Price ?? 0
}).ToList()

            };

            return dto;
        }
    }
}
