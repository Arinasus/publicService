using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Services;
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
    // Проверяем авторизацию
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null)
        return Unauthorized("Нет или неверный токен");

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

    // Проверяем, что счет принадлежит текущему пользователю
    if (invoice.Contract == null || invoice.Contract.UserID != user.UserID)
        return Forbid("Этот счет не принадлежит вам");

    // ищем последнее показание до даты выставления счета
    var previousReadings = await _context.MeterReadings
        .Where(m => m.ContractID == invoice.ContractID && m.ReadingDate < invoice.IssueDate)
        .Include(m => m.Service)
        .GroupBy(m => m.ServiceID)
        .Select(g => g.OrderByDescending(r => r.ReadingDate).FirstOrDefault())
        .ToListAsync();

    // формируем список счётчиков по всем услугам контракта
    var dtoMeters = invoice.Contract.Services.Select(s =>
    {
        var prev = previousReadings.FirstOrDefault(r => r.ServiceID == s.ServiceID);
        return new MeterReadingDto
        {
            ServiceID = s.ServiceID,
            ServiceName = s.ServiceName,
            PreviousValue = prev?.ReadingValue ?? 0,   // если нет показаний → 0
            CurrentValue = 0,                          // пользователь введёт при оплате
            Unit = s.UnitOfMeasure,
            Price = s.Price
        };
    }).ToList();

    var dto = new InvoiceDetailsDto
    {
        InvoiceID = invoice.InvoiceID,
        ContractID = invoice.ContractID,
        IssueDate = invoice.IssueDate,
        DueDate = invoice.DueDate,
        Period = $"{invoice.IssueDate:yyyy-MM}",
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
            Name = invoice.Contract?.User != null
                ? $"{invoice.Contract.User.FirstName} {invoice.Contract.User.LastName}"
                : "Неизвестно",
            PayerNumber = invoice.Contract?.ContractNumber ?? "",
            Address = invoice.Contract?.User != null
                ? invoice.Contract.User.UserAddresses
                    .Where(ua => ua.IsPrimary)
                    .Select(ua => ua.Address != null ? ua.Address.FullAddress : "")
                    .FirstOrDefault() ?? "Адрес не указан"
                : "Адрес не указан"
        },

        Meters = dtoMeters
    };

    return Ok(dto);
}

    }
}
