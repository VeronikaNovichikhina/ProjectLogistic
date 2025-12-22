using LogisticCompany.Domain.Repositories;

public interface ICompanyClientRepository : IRepository<CompanyClient>
{
    Task<CompanyClient?> GetByClientIdAsync(int clientId);
    Task<bool> InnExistsAsync(string inn);
    Task<bool> InnExistsForOtherCompanyAsync(int companyClientId, string inn);
}
