using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.DTO
{
    public class RegisterModel
    {


        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Подтвердите пароль")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
