using LogisticCompany.Domain.Entities.Location;
using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.Domain.Entities.Employee
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? PatronymicName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Position { get; set; } = string.Empty; 

        public int BranchId { get; set; }

        public int? UserId { get; set; } 

        
        public virtual Branch Branch { get; set; } = null!;
        public virtual User? User { get; set; }
    }

    public enum EmployeePosition
    {
        Manager,        
        Courier,
        Administrator
    }
}
