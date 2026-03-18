using LogisticCompany.Application.DTO;
using LogisticCompany.DTO;
using System.Security.Claims;

namespace LogisticCompany.Application.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientAsync();

        Task<CreateClientResult> CreateAsync(CreateClientDto dto);
        Task<List<ClientType>> GetClientTypesAsync();

        Task<ClientDto?> GetByIdAsync(int clientId);
        Task DeleteAsync(int clientId);
        Task UpdateAsync(int clientId, ClientDto clientDto);

        Task<Client?> GetClientByEmailAsync(string email);
        Task<List<Order>> GetOrdersByClientAsync(int clientId);
        Task SaveClientAsync(Client client, IndividualClient? individualClient, CompanyClient? companyClient);

        Task<Client?> GetCurrentClientAsync(ClaimsPrincipal user);
    }
}

