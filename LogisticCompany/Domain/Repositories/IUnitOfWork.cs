namespace LogisticCompany.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IClientRepository Clients { get; }
    IClientTypeRepository ClientTypes { get; }
    IIndividualClientRepository IndividualClients { get; } 
    ICompanyClientRepository CompanyClients { get; } 


    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
