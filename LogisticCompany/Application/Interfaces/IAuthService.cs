using LogisticCompany.Application.DTO;

namespace LogisticCompany.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string email, string password);
        Task<(bool Success, string ErrorMessage)> RegisterAsync(string email, string password);
        Task LogoutAsync();
        Task<(bool IsAuthenticated, string Email, string Role)> GetUserInfoAsync();
    }
}
