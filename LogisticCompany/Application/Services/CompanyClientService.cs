using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Application.Services
{
    public class CompanyClientService : ICompanyClientService
    {
        private readonly AppDbContext _db;

        public CompanyClientService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(CompanyClientDto dto, int clientId)
        {
            if (clientId <= 0)
                throw new Exception("Некорректный клиент");

            if (string.IsNullOrWhiteSpace(dto.CompanyName))
                throw new Exception("Название компании обязательно");

            if (string.IsNullOrWhiteSpace(dto.Inn))
                throw new Exception("ИНН обязателен");

            var client = await _db.Clients
    .Include(c => c.CompanyClients)
    .FirstOrDefaultAsync(c => c.ClientsId == clientId);

            if (client == null)
                throw new Exception("Клиент не найден");

            client.CompanyClients ??= new List<CompanyClient>();

            client.CompanyClients.Add(new CompanyClient
            {
                CompanyName = dto.CompanyName,
                Inn = dto.Inn,
                ClientsId = clientId
            });

            await _db.SaveChangesAsync();

        }
    }

}
