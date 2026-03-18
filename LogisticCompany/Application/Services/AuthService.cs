using LogisticCompany.Application.DTO;
using LogisticCompany.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace LogisticCompany.Application.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;
        public AuthService(AppDbContext db, IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authStateProvider)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return LoginResult.Fail("Неверный логин или пароль");

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });

            return LoginResult.Success(user.Role);
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(string email, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                return (false, "Пользователь с таким Email уже существует.");
            }

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User",
                IsTemporaryPassword = false
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool, string)> GetUserInfoAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
                return (false, string.Empty);

            var name = user.FindFirst("name")?.Value ?? user.Identity.Name ?? "";
            return (true, name);
        }

    }
}
