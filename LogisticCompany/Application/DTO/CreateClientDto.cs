namespace LogisticCompany.Application.DTO
{
    public class CreateClientDto
    {
        public string Phone { get; set; } = string.Empty;


        public string Email { get; set; } = string.Empty;


        public int ClientTypeId { get; set; }
    }
}
