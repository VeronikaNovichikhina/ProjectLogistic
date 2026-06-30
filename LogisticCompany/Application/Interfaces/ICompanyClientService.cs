using LogisticCompany.Application.DTO;

namespace LogisticCompany.Application.Interfaces
{
    public interface ICompanyClientService
    {
        Task CreateAsync(CompanyClientDto dto, int clientId);
    }
}
