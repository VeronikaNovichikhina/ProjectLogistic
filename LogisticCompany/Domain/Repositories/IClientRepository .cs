namespace LogisticCompany.Domain.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> GetWithDetailsAsync(int id);
        Task<Client?> GetWithIndividualDetailsAsync(int id);
        Task<Client?> GetWithCompanyDetailsAsync(int id);
        Task<Client?> GetWithAllRelationsAsync(int id);
        IQueryable<Client> GetQueryable();

        Task<List<Client>> GetByTypeAsync(int clientTypeId);
        Task<List<Client>> GetByPhoneAsync(string phone);
        Task<List<Client>> SearchByNameAsync(string searchTerm);
        Task<List<Client>> GetClientsWithOrdersAsync();

        Task<bool> PhoneExistsAsync(string phone);
        Task<bool> PhoneExistsForOtherClientAsync(int clientId, string phone);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> EmailExistsForOtherClientAsync(int clientId, string email);

        Task<IndividualClient?> GetIndividualClientAsync(int clientId);
        Task<bool> PassportNumberExistsAsync(string passportNumber);
        Task<bool> PassportNumberExistsForOtherClientAsync(int clientId, string passportNumber);

        Task<CompanyClient?> GetCompanyClientAsync(int clientId);
        Task<bool> InnExistsAsync(string inn);
        Task<bool> InnExistsForOtherCompanyAsync(int companyClientId, string inn);

        Task<int> GetTotalCountAsync();
        Task<int> GetCountByTypeAsync(int clientTypeId);
        
        Task CreateRangeAsync(List<Client> clients);
        Task UpdateRangeAsync(List<Client> clients);
    }
}
