using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UtilitiesDbContext _context;

        public AdminController(UtilitiesDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var usersCount = await _context.Users.CountAsync();
            var adminsCount = await _context.Users.CountAsync(u => u.Role == "admin");
            var servicesCount = await _context.Services.CountAsync();
            var unpaidInvoicesCount = await _context.Invoices.CountAsync(i => !i.IsPaid);

            return new
            {
                users = usersCount,
                admins = adminsCount,
                services = servicesCount,
                unpaidInvoices = unpaidInvoicesCount
            };
        }
    }
}
