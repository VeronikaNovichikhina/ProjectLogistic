using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.Application.DTOs.Clients
{
    public class IndividualClientDto
    {
        public int IndividualClientId { get; set; }

       
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string? PatronymicName { get; set; }
        public string PassportNumber { get; set; } = string.Empty;

        public DateTime? PassportDateOfIssue { get; set; }

        public bool IsPassportExpired =>
         PassportDateOfIssue.HasValue && PassportDateOfIssue.Value < DateTime.Today;

        public int DaysUntilExpiry =>
            PassportDateOfIssue.HasValue ?
            (PassportDateOfIssue.Value - DateTime.Today).Days : -1;
    }
}
