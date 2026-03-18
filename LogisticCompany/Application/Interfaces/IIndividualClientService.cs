using LogisticCompany.DTO;

namespace LogisticCompany.Application.Interfaces
{
    public interface IIndividualClientService
    {
        Task CreateAsync(IndividualClientDto dto, int clientId);
    }
}
