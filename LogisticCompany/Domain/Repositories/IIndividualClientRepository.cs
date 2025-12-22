using LogisticCompany.Domain.Repositories;

public interface IIndividualClientRepository : IRepository<IndividualClient>
{
    Task<IndividualClient?> GetByClientIdAsync(int clientId);
    Task<bool> PassportNumberExistsAsync(string passportNumber);
    Task<bool> PassportNumberExistsForOtherClientAsync(int clientId, string passportNumber);
}
