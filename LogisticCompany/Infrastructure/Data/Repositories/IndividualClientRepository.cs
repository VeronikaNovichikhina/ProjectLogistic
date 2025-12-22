
using LogisticCompany.Db;
using LogisticCompany.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class IndividualClientRepository : BaseRepository<IndividualClient>, IIndividualClientRepository
{
    public IndividualClientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IndividualClient?> GetByClientIdAsync(int clientId)
    {
        return await _context.IndividualClients
            .FirstOrDefaultAsync(i => i.ClientsId == clientId);
    }

    public async Task<bool> PassportNumberExistsAsync(string passportNumber)
    {
        return await _context.IndividualClients
            .AnyAsync(i => i.PassportNumber == passportNumber);
    }

    public async Task<bool> PassportNumberExistsForOtherClientAsync(int clientId, string passportNumber)
    {
        return await _context.IndividualClients
            .AnyAsync(i => i.ClientsId != clientId && i.PassportNumber == passportNumber);
    }
}
