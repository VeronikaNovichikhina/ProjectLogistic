
using global::LogisticCompany.Db;
using LogisticCompanyApi.Service.LogisticCompany.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext db, JwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email и пароль обязательны");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

            if (user == null)
                return Unauthorized("Неверный email или пароль");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                return Unauthorized("Аккаунт временно заблокирован");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                await _db.SaveChangesAsync();
                return Unauthorized("Неверный email или пароль");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Role,
                IsTemporaryPassword = user.IsTemporaryPassword
            });
        }
    }

    public record LoginRequest(string Email, string Password);

    public record LoginResponse
    {
        public string Token { get; init; } = "";
        public string Email { get; init; } = "";
        public string Role { get; init; } = "";
        public bool IsTemporaryPassword { get; init; }
    }
}