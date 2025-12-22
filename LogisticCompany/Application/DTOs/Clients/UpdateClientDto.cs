using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.Application.DTOs.Clients
{
    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Телефон обязателен")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;
    }
    public class UpdateIndividualClientDto
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов")]
        public string? PatronymicName { get; set; }

        [Required(ErrorMessage = "Паспорт обязателен")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Паспорт должен содержать 12 символов")]
        public string PassportNumber { get; set; } = string.Empty;
    }

    public class UpdateCompanyClientDto
    {
        [Required(ErrorMessage = "Название компании обязательно")]
        [StringLength(200, ErrorMessage = "Название компании не должно превышать 200 символов")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ИИН/БИН обязателен")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "ИИН/БИН должен содержать 12 символов")]
        public string Inn { get; set; } = string.Empty;
    }
}
