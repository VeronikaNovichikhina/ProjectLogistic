using LogisticCompany.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticCompany.Api.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IUserService _userService;

        public ProfileController(IClientService clientService, IUserService userService)
        {
            _clientService = clientService;
            _userService = userService;
        }

        // Получить профиль текущего пользователя
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var email = GetCurrentUserEmail();
                if (string.IsNullOrEmpty(email)) return Unauthorized();

                var client = await _clientService.GetClientByEmailAsync(email);
                if (client == null) return NotFound("Профиль не найден");

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Сменить пароль
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var email = GetCurrentUserEmail();
                if (string.IsNullOrEmpty(email)) return Unauthorized();

                await _userService.ChangePasswordAsync(
                    email,
                    request.CurrentPassword,
                    request.NewPassword);

                return Ok("Пароль успешно изменён");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }
    }

    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
