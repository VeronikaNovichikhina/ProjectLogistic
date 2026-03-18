using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.DTO;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Application.Services
{
    public class IndividualClientService : IIndividualClientService
    {
        private readonly AppDbContext _db;

        public IndividualClientService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(IndividualClientDto dto, int clientId)
        {
            if (clientId <= 0)
                throw new Exception("Некорректный клиент");

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                throw new Exception("Имя и фамилия обязательны");

            if (!dto.PassportDateOfIssue.HasValue)
                throw new Exception("Дата выдачи паспорта обязательна");

            if (dto.IsPassportExpired)
                throw new Exception("Паспорт просрочен");

            var client = await _db.Clients
    .Include(c => c.IndividualClients)
    .FirstOrDefaultAsync(c => c.ClientsId == clientId);

            if (client == null)
                throw new Exception("Клиент не найден");

            client.IndividualClients ??= new List<IndividualClient>();

            client.IndividualClients.Add(new IndividualClient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PatronymicName = dto.PatronymicName,
                PassportNumber = dto.PassportNumber,
                PassportDateOfIssue =
                    DateOnly.FromDateTime(dto.PassportDateOfIssue.Value),
                ClientsId = clientId
            });


            await _db.SaveChangesAsync();

        }
    }

}
