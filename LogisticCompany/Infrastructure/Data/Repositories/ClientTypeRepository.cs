using LogisticCompany.Db;
using LogisticCompany.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Infrastructure.Data.Repositories
{
    public class ClientTypeRepository : BaseRepository<ClientType>, IClientTypeRepository
    {
        public ClientTypeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ClientType?> GetByNameAsync(string typeName)
        {
            return await _context.ClientTypes
                .FirstOrDefaultAsync(ct => ct.TypeName == typeName);
        }
        public override async Task<bool> ExistsAsync(int id)
        {
            return await _context.ClientTypes
                .AnyAsync(ct => ct.ClientTypeId == id);
        }
    }
}
