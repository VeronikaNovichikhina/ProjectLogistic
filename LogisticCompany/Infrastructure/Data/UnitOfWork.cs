using Infrastructure.Data.Repositories;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Repositories;
using LogisticCompany.Infrastructure.Data.Repositories;
using static NuGet.Packaging.PackagingConstants;

namespace Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Clients = new ClientRepository(_context);
        ClientTypes = new ClientTypeRepository(_context);
        IndividualClients = new IndividualClientRepository(_context); // ДОБАВЬ
        CompanyClients = new CompanyClientRepository(_context); // ДОБАВЬ

        
    }

    public IClientRepository Clients { get; }
    public IClientTypeRepository ClientTypes { get; }

    public IIndividualClientRepository IndividualClients { get; } // ДОБАВЬ
    public ICompanyClientRepository CompanyClients { get; } // ДОБАВЬ

    
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Task BeginTransactionAsync()
    {
        // Реализация транзакции если нужно
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        // Реализация коммита транзакции
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        // Реализация отката транзакции
        return Task.CompletedTask;
    }
}