using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace LogisticCompany.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthService> _logger;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const int MinPasswordLength = 8;

        public AuthService(
            AppDbContext db,
            IHttpContextAccessor httpContextAccessor,
            AuthenticationStateProvider authStateProvider,
            ILogger<AuthService> logger)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            var ip = GetClientIp();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Попытка входа с пустыми данными. IP: {IP}", ip);
                return LoginResult.Fail("Неверный логин или пароль");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("Вход: email не найден. IP: {IP}", ip);
                return LoginResult.Fail("Неверный логин или пароль");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var remaining = (int)(user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes + 1;
                _logger.LogWarning("Вход заблокирован: {Email}. IP: {IP}", email, ip);
                return LoginResult.Fail($"Аккаунт заблокирован. Повторите через {remaining} мин.");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    _logger.LogWarning("Аккаунт заблокирован после {N} попыток: {Email}. IP: {IP}",
                        MaxFailedAttempts, email, ip);
                }
                await _db.SaveChangesAsync();
                return LoginResult.Fail("Неверный логин или пароль");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name,           user.Email),
                new(ClaimTypes.Role,           user.Role),
                new("IsTemporaryPassword",     user.IsTemporaryPassword.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            _logger.LogInformation("Успешный вход: {Email}, роль: {Role}, IP: {IP}", user.Email, user.Role, ip);
            return LoginResult.Success(user.Role);
        }


        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Некорректный адрес email.");

            if (password.Length < MinPasswordLength)
                return (false, $"Пароль должен быть не менее {MinPasswordLength} символов.");

            if (!password.Any(char.IsUpper))
                return (false, "Пароль должен содержать хотя бы одну заглавную букву.");

            if (!password.Any(char.IsDigit))
                return (false, "Пароль должен содержать хотя бы одну цифру.");

            var normalizedEmail = email.Trim().ToLowerInvariant();
            if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
                return (false, "Пользователь с таким email уже существует.");

            var user = new User
            {
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
                Role = "User",
                IsTemporaryPassword = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Зарегистрирован новый пользователь: {Email}", normalizedEmail);
            return (true, null);
        }


        public async Task LogoutAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User
                            .FindFirst(ClaimTypes.Name)?.Value;

            await _httpContextAccessor.HttpContext!.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Выход: {Email}", email ?? "неизвестно");
        }

        public async Task<(bool IsAuthenticated, string Email, string Role)> GetUserInfoAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var principal = authState.User;

            if (principal.Identity?.IsAuthenticated != true)
                return (false, string.Empty, string.Empty);

            var email = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            return (true, email, role);
        }


        private string GetClientIp()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? context?.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";
        }

        
    }
}
