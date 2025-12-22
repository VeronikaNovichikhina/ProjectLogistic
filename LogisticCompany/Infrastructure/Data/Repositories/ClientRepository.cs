using LogisticCompany.Db;
using LogisticCompany.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Infrastructure.Data.Repositories
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }
        public IQueryable<Client> GetQueryable()
        {
            return _context.Clients.AsQueryable();
        }
        public override async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .FirstOrDefaultAsync(c => c.ClientsId == id);
        }
        public override async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .OrderBy(c => c.ClientsId)
                .ToListAsync();
        }
        public async Task<Client?> GetWithDetailsAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .FirstOrDefaultAsync(c => c.ClientsId == id);
        }
        public async Task<Client?> GetWithIndividualDetailsAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .Include(c => c.IndividualClients)
                .FirstOrDefaultAsync(c => c.ClientsId == id);
        }

        public async Task<Client?> GetWithCompanyDetailsAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .Include(c => c.CompanyClients)
                .FirstOrDefaultAsync(c => c.ClientsId == id);
        }

        public async Task<Client?> GetWithAllRelationsAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.ClientsId == id);
        }

        public async Task<List<Client>> GetByTypeAsync(int clientTypeId)
        {
            return await _context.Clients
                .Include(c => c.ClientType)
                .Where(c => c.ClientTypeId == clientTypeId)
                .ToListAsync();
        }

        public async Task<List<Client>> GetByPhoneAsync(string phone)
        {
            return await _context.Clients
                .Where(c => c.Phone.Contains(phone))
                .ToListAsync();
        }

        public async Task<List<Client>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Clients
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .Where(c =>
                    c.IndividualClients.Any(i =>
                        i.FirstName.Contains(searchTerm) ||
                        i.LastName.Contains(searchTerm) ||
                        i.PatronymicName.Contains(searchTerm)) ||
                    c.CompanyClients.Any(co =>
                        co.CompanyName.Contains(searchTerm))
                )
                .ToListAsync();
        }

        public async Task<List<Client>> GetClientsWithOrdersAsync()
        {
            return await _context.Clients
                .Include(c => c.Orders)
                .Where(c => c.Orders.Any())
                .ToListAsync();
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Clients
                .AnyAsync(c => c.Phone == phone);
        }

        public async Task<bool> PhoneExistsForOtherClientAsync(int clientId, string phone)
        {
            return await _context.Clients
                .AnyAsync(c => c.ClientsId != clientId && c.Phone == phone);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Clients
                .AnyAsync(c => c.Email == email);
        }

        public async Task<bool> EmailExistsForOtherClientAsync(int clientId, string email)
        {
            return await _context.Clients
                .AnyAsync(c => c.ClientsId != clientId && c.Email == email);
        }

        public async Task<IndividualClient?> GetIndividualClientAsync(int clientId)
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

        public async Task<CompanyClient?> GetCompanyClientAsync(int clientId)
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

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Clients.CountAsync();
        }

        public async Task<int> GetCountByTypeAsync(int clientTypeId)
        {
            return await _context.Clients
                .CountAsync(c => c.ClientTypeId == clientTypeId);
        }

        public async Task CreateRangeAsync(List<Client> clients)
        {
            await _context.Clients.AddRangeAsync(clients);
        }

        public async Task UpdateRangeAsync(List<Client> clients)
        {
            _context.Clients.UpdateRange(clients);
            await Task.CompletedTask;
        }
    }
}
