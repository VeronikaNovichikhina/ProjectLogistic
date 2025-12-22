using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.DTO
{
    public class CompanyClientDto
    {
        public int CompanyClientId { get; set; }

        [Required(ErrorMessage = "Название компании обязательно")]
        [StringLength(200, ErrorMessage = "Название компании не должно превышать 200 символов")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ИИН/БИН обязателен")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "ИИН/БИН должен содержать 12 символов")]
        public string Inn { get; set; } = string.Empty;
    }
}
