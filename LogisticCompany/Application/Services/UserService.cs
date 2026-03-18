using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace LogisticCompany.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IJSRuntime _js;

        public UserService(AppDbContext db, IJSRuntime js)
        {
            _db = db;
            _js = js;
        }

        public async Task<User?> GetCurrentUserAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsTemporaryPasswordAsync(string email)
        {
            try
            {
                var user = await GetCurrentUserAsync(email);
                return user?.IsTemporaryPassword == true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> HasChangedPasswordCookieAsync()
        {
            try
            {
                var value = await _js.InvokeAsync<string>("eval",
                    "document.cookie.replace(/(?:(?:^|.*;\\s*)userPasswordChanged\\s*\\=\\s*([^;]*).*$)|^.*$/, '$1')");
                return !string.IsNullOrEmpty(value) && value != "dismissed";
            }
            catch
            {
                return false;
            }
        }

        public async Task ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user == null) throw new Exception("Пользователь не найден");
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new Exception("Текущий пароль неверен");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
        }
    }
}
