namespace LogisticCompany.Application.DTO
{
    public class ClientDto
    {
        public int ClientsId { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ClientTypeId { get; set; }
        public string ClientTypeName { get; set; } = string.Empty;
        public IndividualClientDto? IndividualClient { get; set; }
        public CompanyClientDto? CompanyClient { get; set; }
        public int? UserId { get; set; }
    }
}
