using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace LogisticCompany.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _db;
        public ClientService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Client>> GetAllClientAsync()
        {
          return await _db.Clients
                .Include(c => c.ClientType)
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .ToListAsync();
        }
        public async Task<List<ClientType>> GetClientTypesAsync()
        {
            return await _db.ClientTypes.ToListAsync();
        }

        public async Task<CreateClientResult> CreateAsync(CreateClientDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Phone))
                throw new Exception("Телефон обязателен");

            if (await _db.Clients.AnyAsync(c => c.Phone == dto.Phone))
                throw new Exception("Клиент с таким телефоном уже существует");

            var client = new Client
            {
                Phone = dto.Phone,
                Email = dto.Email,
                ClientTypeId = dto.ClientTypeId
            };

            _db.Clients.Add(client);
            await _db.SaveChangesAsync();

            return new CreateClientResult
            {
                ClientId = client.ClientsId,
                ClientTypeId = client.ClientTypeId
            };
        }

        public async Task<ClientDto?> GetByIdAsync(int clientId)
        {
            var clientEntity = await _db.Clients
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .FirstOrDefaultAsync(c => c.ClientsId == clientId);

            if (clientEntity == null) return null;

            var individual = clientEntity.IndividualClients.FirstOrDefault();
            var company = clientEntity.CompanyClients.FirstOrDefault();

            return new ClientDto
            {
                ClientsId = clientEntity.ClientsId,
                Phone = clientEntity.Phone,
                Email = clientEntity.Email,
                ClientTypeId = clientEntity.ClientTypeId,
                IndividualClient = individual != null
                    ? new IndividualClientDto
                    {
                        FirstName = individual.FirstName,
                        LastName = individual.LastName,
                        PatronymicName = individual.PatronymicName,
                        PassportNumber = individual.PassportNumber,
                        PassportDateOfIssue = individual.PassportDateOfIssue?.ToDateTime(TimeOnly.MinValue)
                    }
                    : null,
                CompanyClient = company != null
                    ? new CompanyClientDto
                    {
                        CompanyName = company.CompanyName,
                        Inn = company.Inn
                    }
                    : null
            };
        }


        public async Task DeleteAsync(int clientId)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientsId == clientId);
            if (client == null) throw new Exception("Клиент не найден");

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateAsync(int clientId, ClientDto clientDto)
        {
            var clientEntity = await _db.Clients
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .FirstOrDefaultAsync(c => c.ClientsId == clientId);

            if (clientEntity == null)
                throw new Exception("Клиент не найден");

            clientEntity.Phone = clientDto.Phone;
            clientEntity.Email = clientDto.Email;

            if (clientDto.ClientTypeId == 1 && clientDto.IndividualClient != null)
            {
                var individual = clientEntity.IndividualClients.FirstOrDefault() ?? new IndividualClient();
                if (!clientEntity.IndividualClients.Contains(individual))
                    clientEntity.IndividualClients.Add(individual);

                individual.FirstName = clientDto.IndividualClient.FirstName;
                individual.LastName = clientDto.IndividualClient.LastName;
                individual.PatronymicName = clientDto.IndividualClient.PatronymicName;
                individual.PassportNumber = clientDto.IndividualClient.PassportNumber;
                individual.PassportDateOfIssue = clientDto.IndividualClient.PassportDateOfIssue.HasValue
                    ? DateOnly.FromDateTime(clientDto.IndividualClient.PassportDateOfIssue.Value)
                    : null;
            }
            else if (clientDto.ClientTypeId == 2 && clientDto.CompanyClient != null)
            {
                var company = clientEntity.CompanyClients.FirstOrDefault() ?? new CompanyClient();
                if (!clientEntity.CompanyClients.Contains(company))
                    clientEntity.CompanyClients.Add(company);

                company.CompanyName = clientDto.CompanyClient.CompanyName;
                company.Inn = clientDto.CompanyClient.Inn;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Client?> GetClientByEmailAsync(string email)
        {
            return await _db.Clients
                .Include(c => c.ClientType)
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<List<Order>> GetOrdersByClientAsync(int clientId)
        {
            return await _db.Orders
            .Where(o => o.ClientsId == clientId)
            .Include(o => o.DeliveryType)
            .Include(o => o.TransportType)
            .Include(o => o.OriginTown)
                .ThenInclude(t => t.Country)
            .Include(o => o.DestinationTown)
                .ThenInclude(t => t.Country)
            .Include(o => o.PickupBranches)
            .Include(o => o.Trackings)
                .ThenInclude(t => t.Status)
            .Select(o => new Order
            {
                OrdersId = o.OrdersId,
                OrderNumber = o.OrderNumber,
                DescriptionParcel = o.DescriptionParcel,
                LengthCm = o.LengthCm,
                WidthCm = o.WidthCm,
                HeightCm = o.HeightCm,
                Weight = o.Weight,
                DeliveryType = o.DeliveryType,
                TransportType = o.TransportType,
                OriginTown = o.OriginTown,
                DestinationTown = o.DestinationTown,
                PickupBranches = o.PickupBranches,
                Trackings = o.Trackings
            })
            .OrderByDescending(o => o.OrdersId)
            .ToListAsync();
        }

        public async Task SaveClientAsync(Client client, IndividualClient? individualClient, CompanyClient? companyClient)
        {

            if (client.ClientsId == 0)
                _db.Clients.Add(client);
            else
                _db.Clients.Update(client);

            if (client.ClientTypeId == 1 && individualClient != null)
            {
                individualClient.Clients = client;
                if (individualClient.IndividualId == 0)
                    _db.IndividualClients.Add(individualClient);
                else
                    _db.IndividualClients.Update(individualClient);
            }
            else if (client.ClientTypeId == 2 && companyClient != null)
            {
                companyClient.Clients = client;
                if (companyClient.CompanyId == 0)
                    _db.CompanyClients.Add(companyClient);
                else
                    _db.CompanyClients.Update(companyClient);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Client?> GetCurrentClientAsync(ClaimsPrincipal user)
        {
            if (user.Identity == null || !user.Identity.IsAuthenticated)
                return null;


            var email = user.Identity.Name?.ToLower() ?? string.Empty;

            var client = await _db.Clients
                .Include(c => c.IndividualClients)
                .Include(c => c.CompanyClients)
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email);

            return client;
        }
    }
}
