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

        public UserService(AppDbContext db)
        {
            _db = db;
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


        public async Task ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower())
                ?? throw new Exception("Пользователь не найден");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new Exception("Текущий пароль неверен");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            user.IsTemporaryPassword = false;
            await _db.SaveChangesAsync();
        }
    }
}
