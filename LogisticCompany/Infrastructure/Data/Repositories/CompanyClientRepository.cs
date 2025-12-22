
using LogisticCompany.Db;
using LogisticCompany.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class CompanyClientRepository : BaseRepository<CompanyClient>, ICompanyClientRepository
{
    public CompanyClientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<CompanyClient?> GetByClientIdAsync(int clientId)
    {
        return await _context.CompanyClients
            .FirstOrDefaultAsync(c => c.ClientsId == clientId);
    }

    public async Task<bool> InnExistsAsync(string inn)
    {
        return await _context.CompanyClients
            .AnyAsync(c => c.Inn == inn);
    }

    public async Task<bool> InnExistsForOtherCompanyAsync(int companyClientId, string inn)
    {
        return await _context.CompanyClients
            .AnyAsync(c => c.CompanyId != companyClientId && c.Inn == inn);
    }
}
