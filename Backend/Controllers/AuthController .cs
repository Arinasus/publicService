using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore; // для AnyAsync и FirstOrDefaultAsync
using BCrypt.Net; // для хэширования паролей

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UtilitiesDbContext _context;

    public AuthController(UtilitiesDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Пользователь с таким email уже существует");

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "user"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = Guid.NewGuid().ToString();
        _context.AuthTokens.Add(new AuthToken { Token = token, UserID = user.UserID });
        await _context.SaveChangesAsync();

        return Ok(new { token, role = user.Role });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Неверный email или пароль");

        var token = Guid.NewGuid().ToString();
        _context.AuthTokens.Add(new AuthToken { Token = token, UserID = user.UserID });
        await _context.SaveChangesAsync();

        return Ok(new { token, role = user.Role });
    }
}
