using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using AutoMapper;
using Backend.Profiles;
using Backend.Services;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;
        private readonly IMapper _mapper;
        public ContractsController(UtilitiesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Contracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContracts()
        {
            var contracts = await _context.Contracts
               .Include(c => c.User)
               .Include(c => c.Address)
               .Include(c => c.Services)
               .Include(c => c.Provider)
               .ToListAsync();
            return _mapper.Map<List<ContractDto>>(contracts);
        }
        [HttpGet("me")]
public async Task<ActionResult<IEnumerable<ContractDto>>> GetMyContracts()
{
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null) return Unauthorized("Нет или неверный токен");

    var contracts = await _context.Contracts
        .Include(c => c.User)
        .Include(c => c.Address)
        .Include(c => c.Services)
        .Include(c => c.Provider)
        .Where(c => c.UserID == user.UserID)
        .ToListAsync();

    return _mapper.Map<List<ContractDto>>(contracts);
}

        // GET: api/Contracts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDto>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.User)
                .Include(c => c.Address)
                .Include(c => c.Services)
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(c => c.ContractID == id);

            if (contract == null)
            {
                return NotFound();
            }

            return _mapper.Map<ContractDto>(contract);
        }

        [HttpPost]
public async Task<IActionResult> CreateContract([FromBody] ContractCreateDto dto)
{
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null) return Unauthorized("Нет или неверный токен");

    // Находим основной адрес пользователя
    var primaryAddress = await _context.UserAddresses
        .Include(ua => ua.Address)
        .FirstOrDefaultAsync(ua => ua.UserID == user.UserID && ua.IsPrimary);

    if (primaryAddress == null)
        return BadRequest("У пользователя нет основного адреса. Добавьте адрес в профиле.");

    var contract = new Contract
    {
        ContractNumber = dto.ContractNumber,
        ContractStartDate = dto.ContractStartDate,
        ContractEndDate = dto.ContractEndDate,
        UserID = user.UserID,
        AddressID = primaryAddress.AddressID, // ✅ сохраняем основной адрес
        ProviderID = dto.ProviderID
    };

    _context.Contracts.Add(contract);
    await _context.SaveChangesAsync();

    // Привязываем услуги
    var services = await _context.Services
        .Where(s => dto.ServiceIds.Contains(s.ServiceID))
        .ToListAsync();

    foreach (var service in services)
        service.ContractID = contract.ContractID;

    await _context.SaveChangesAsync();

    // Создаём счёт
    var invoice = new Invoice
    {
        ContractID = contract.ContractID,
        IssueDate = DateTime.UtcNow,
        DueDate = DateTime.UtcNow.AddDays(7),
        Period = DateTime.UtcNow.ToString("yyyy-MM"),
        TotalAmount = services.Sum(s => s.Price),
        IsPaid = false
    };

    _context.Invoices.Add(invoice);
    await _context.SaveChangesAsync();

    // Уведомление
    var notification = new Notification
    {
        UserID = user.UserID,
        NotificationDate = DateTime.UtcNow,
        NotificationType = "Info",
        NotificationText = $"Контракт {contract.ContractNumber} создан. Счёт №{invoice.InvoiceID} на сумму {invoice.TotalAmount} BYN.",
        IsRead = false
    };

    _context.Notifications.Add(notification);
    await _context.SaveChangesAsync();

    // Загружаем контракт с услугами
    var contractWithServices = await _context.Contracts
        .Include(c => c.Services)
        .Include(c => c.User)
        .Include(c => c.Address)
        .Include(c => c.Provider)
        .FirstOrDefaultAsync(c => c.ContractID == contract.ContractID);

    return Ok(new
    {
        contract = _mapper.Map<ContractDto>(contractWithServices),
        invoice = new
        {
            invoice.InvoiceID,
            invoice.TotalAmount,
            invoice.DueDate
        }
    });
}


[HttpPost("{contractId}/add-service")]
public async Task<IActionResult> AddServiceToContract(int contractId, [FromBody] AddServiceToContractDto dto)
{
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null) return Unauthorized("Нет или неверный токен");

    var contract = await _context.Contracts
        .Include(c => c.Services)
        .Include(c => c.Provider)
        .Include(c => c.Address)
        .FirstOrDefaultAsync(c => c.ContractID == contractId && c.UserID == user.UserID);

    if (contract == null)
        return NotFound("Контракт не найден или не принадлежит вам");

    var service = await _context.Services
        .Include(s => s.Provider)
        .FirstOrDefaultAsync(s => s.ServiceID == dto.ServiceID);

    if (service == null)
        return NotFound("Услуга не найдена");

    if (service.ProviderID != contract.ProviderID)
        return BadRequest($"Эта услуга принадлежит другому провайдеру ({service.Provider?.ProviderName})");

    if (service.ContractID.HasValue && service.ContractID == contractId)
        return BadRequest("Эта услуга уже добавлена в контракт");

    service.ContractID = contractId;

    var invoice = new Invoice
    {
        ContractID = contractId,
        IssueDate = DateTime.UtcNow,
        DueDate = DateTime.UtcNow.AddDays(14),
        Period = DateTime.UtcNow.ToString("yyyy-MM"),
        TotalAmount = service.Price,
        IsPaid = false
    };

    _context.Invoices.Add(invoice);
    await _context.SaveChangesAsync();

    var notification = new Notification
    {
        UserID = user.UserID,
        NotificationDate = DateTime.UtcNow,
        NotificationType = "Info",
        NotificationText = $"Услуга '{service.ServiceName}' добавлена в контракт {contract.ContractNumber}. Счёт №{invoice.InvoiceID} на сумму {service.Price} BYN.",
        IsRead = false,
        LastUpdatedDate = DateTime.UtcNow
    };

    _context.Notifications.Add(notification);
    await _context.SaveChangesAsync();

    // Загружаем контракт заново с услугами
    var updatedContract = await _context.Contracts
        .Include(c => c.Services)
        .Include(c => c.Provider)
        .Include(c => c.Address)
        .FirstOrDefaultAsync(c => c.ContractID == contractId);

    return Ok(new
    {
        contract = _mapper.Map<ContractDto>(updatedContract),
        invoice = new
        {
            invoice.InvoiceID,
            invoice.TotalAmount,
            invoice.DueDate,
            invoice.Period
        }
    });
}


        [HttpDelete("{contractId}/remove-service/{serviceId}")]
public async Task<IActionResult> RemoveServiceFromContract(int contractId, int serviceId)
{
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null) return Unauthorized("Нет или неверный токен");

    var contract = await _context.Contracts
        .Include(c => c.Services)
        .FirstOrDefaultAsync(c => c.ContractID == contractId && c.UserID == user.UserID);

    if (contract == null)
        return NotFound("Контракт не найден или не принадлежит вам");

    var service = contract.Services.FirstOrDefault(s => s.ServiceID == serviceId);
    if (service == null)
        return NotFound("Услуга не найдена в контракте");

    service.ContractID = null; // отвязываем услугу

    await _context.SaveChangesAsync();

    // создаём уведомление
    var notification = new Notification
    {
        UserID = user.UserID,
        NotificationDate = DateTime.UtcNow,
        NotificationType = "Info",
        NotificationText = $"Услуга '{service.ServiceName}' удалена из контракта {contract.ContractNumber}.",
        IsRead = false
    };

    _context.Notifications.Add(notification);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Услуга успешно удалена" });
}


        // PUT: api/Contracts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, ContractCreateDto contractDto) // Измените здесь тоже
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = _mapper.Map<Contract>(contractDto);
            contract.ContractID = id;

            _context.Entry(contract).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
[HttpDelete("{contractId}")]
public async Task<IActionResult> DeleteContract(int contractId)
{
    var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
    if (user == null) 
        return Unauthorized("Нет или неверный токен");

    // Загружаем контракт с услугами
    var contract = await _context.Contracts
        .Include(c => c.Services)
        .FirstOrDefaultAsync(c => c.ContractID == contractId && c.UserID == user.UserID);

    if (contract == null)
        return NotFound("Контракт не найден или не принадлежит вам");

    // Отвязываем услуги
    foreach (var service in contract.Services)
        service.ContractID = null;

    // Удаляем счета напрямую
    var invoices = await _context.Invoices
        .Where(i => i.ContractID == contractId)
        .ToListAsync();

    _context.Invoices.RemoveRange(invoices);

    // Удаляем сам контракт
    _context.Contracts.Remove(contract);

    // Создаём уведомление
    var notification = new Notification
    {
        UserID = user.UserID,
        NotificationDate = DateTime.UtcNow,
        NotificationType = "Info",
        NotificationText = $"Контракт {contract.ContractNumber} был удалён вместе с услугой.",
        IsRead = false
    };

    _context.Notifications.Add(notification);

    await _context.SaveChangesAsync();

    return Ok(new { message = "Контракт и услуга удалены" });
}
}
        public class AddServiceToContractDto
    {
        public int ServiceID { get; set; }
    }
}
