using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.DTO
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(8, ErrorMessage = "Пароль должен быть не менее 8 символов")]
        public string Password { get; set; } = string.Empty;
    }
}
