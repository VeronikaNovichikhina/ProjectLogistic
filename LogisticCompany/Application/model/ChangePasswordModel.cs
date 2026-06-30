using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.Application.model
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "Новый пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Пароль должен содержать заглавные и строчные буквы, а также цифры")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = "";
    }
}
