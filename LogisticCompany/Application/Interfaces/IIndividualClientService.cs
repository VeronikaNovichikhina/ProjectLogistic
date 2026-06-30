using LogisticCompany.Application.DTO;

namespace LogisticCompany.Application.Interfaces
{
    public interface IIndividualClientService
    {
        Task CreateAsync(IndividualClientDto dto, int clientId);
    }
}
