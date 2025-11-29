using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using AutoMapper;
using Backend.Profiles;

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
               .Include(c => c.Service)
               .Include(c => c.Provider)
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
                .Include(c => c.Service)
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(c => c.ContractID == id);

            if (contract == null)
            {
                return NotFound();
            }

            return _mapper.Map<ContractDto>(contract);
        }

        // POST: api/Contracts
        // POST: api/Contracts
        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] ContractCreateDto contractDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = _mapper.Map<Contract>(contractDto);
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            // Для возврата созданного контракта получаем его с включенными связанными данными
            var createdContract = await _context.Contracts
                .Include(c => c.User)
                .Include(c => c.Address)
                .Include(c => c.Service)
                .Include(c => c.Provider)
                .FirstOrDefaultAsync(c => c.ContractID == contract.ContractID);

            return CreatedAtAction(nameof(GetContract), new { id = contract.ContractID }, _mapper.Map<ContractDto>(createdContract));
        }

        // PUT: api/Contracts/5
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
