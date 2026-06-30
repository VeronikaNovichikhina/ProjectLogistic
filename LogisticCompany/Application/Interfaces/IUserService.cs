using LogisticCompany.Domain.Entities.Employee;

namespace LogisticCompany.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync(string email);
        Task<bool> IsTemporaryPasswordAsync(string email);
        Task ChangePasswordAsync(string email, string currentPassword, string newPassword);

    }
}

