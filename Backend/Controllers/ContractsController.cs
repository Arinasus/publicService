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
            // ИСПРАВЛЕНО: используем AuthHelpers вместо GetUserIdFromToken
            var user = await AuthHelpers.GetUserByBearerTokenAsync(_context, Request);
            if (user == null) return Unauthorized("Нет или неверный токен");

            // Сначала создаем контракт без услуг
            var contract = new Contract
            {
                ContractNumber = dto.ContractNumber,
                ContractStartDate = dto.ContractStartDate,
                ContractEndDate = dto.ContractEndDate,
                UserID = user.UserID, // Используем user.UserID
                AddressID = dto.AddressID,
                ProviderID = dto.ProviderID
            };

            // Сохраняем контракт, чтобы получить ContractID
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            // Теперь привязываем услуги к созданному контракту
            var services = await _context.Services
                .Where(s => dto.ServiceIds.Contains(s.ServiceID))
                .ToListAsync();
            
            foreach (var service in services)
            {
                service.ContractID = contract.ContractID;
            }

            // Сохраняем изменения в услугах
            await _context.SaveChangesAsync();

            // Загружаем контракт с услугами для ответа
            var contractWithServices = await _context.Contracts
                .Include(c => c.Services)
                .Include(c => c.User)
                .Include(c => c.Address)
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(c => c.ContractID == contract.ContractID);

            return Ok(_mapper.Map<ContractDto>(contractWithServices));
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

        // DELETE: api/Contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
