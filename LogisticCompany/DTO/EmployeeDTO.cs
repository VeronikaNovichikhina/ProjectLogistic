using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.DTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        public string LastName { get; set; } = string.Empty;

        public string? PatronymicName { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефон обязателен")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Должность обязательна")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Филиал обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите филиал")]
        public int BranchId { get; set; }

        public int TownId { get; set; }

        public int CountryId { get; set; }
        public bool CreateUserAccount { get; set; } = true;
    }

    public class EmployeeDetailsDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public string TownName { get; set; } = string.Empty;
        
    }
}
