using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.Application.DTOs.Clients
{
    public class CreateClientDto
    {
        
        public string Phone { get; set; } = string.Empty;

        
        public string Email { get; set; } = string.Empty;

       
        public int ClientTypeId { get; set; }
    }
}
