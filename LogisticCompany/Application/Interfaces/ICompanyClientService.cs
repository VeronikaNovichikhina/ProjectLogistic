using LogisticCompany.DTO;

namespace LogisticCompany.Application.Interfaces
{
    public interface ICompanyClientService
    {
        Task CreateAsync(CompanyClientDto dto, int clientId);
    }
}
