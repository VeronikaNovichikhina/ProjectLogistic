
using Application.Common;
using LogisticCompany.Application.DTOs.Clients;

namespace LogisticCompany.Application.Interfaces
{
    public interface IClientService
    {
        Task<Result<ClientDto>> CreateClientAsync(CreateClientDto createDto);
        Task<Result<ClientDto>> GetClientByIdAsync(int id);
        Task<Result<List<ClientDto>>> GetAllClientsAsync();
        Task<Result<ClientDto>> UpdateClientAsync(int id, UpdateClientDto updateDto);
        Task<Result<IndividualClientDto>> UpdateIndividualClientAsync(int clientId, UpdateIndividualClientDto updateDto);
        Task<Result<CompanyClientDto>> UpdateCompanyClientAsync(int clientId, UpdateCompanyClientDto updateDto);
        Task<Result> DeleteClientAsync(int id);
        Task<Result<IndividualClientDto>> CreateIndividualClientAsync(int clientId, IndividualClientDto individualDto);
        Task<Result<CompanyClientDto>> CreateCompanyClientAsync(int clientId, CompanyClientDto companyDto);
        Task<Result<List<ClientTypeDto>>> GetClientTypesAsync();
    }
}
