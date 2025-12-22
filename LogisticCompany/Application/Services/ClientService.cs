using Application.Common;
using LogisticCompany.Application.DTOs.Clients;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IClientTypeRepository _clientTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientService> _logger;
    private readonly IIndividualClientRepository _individualClientRepository;
    private readonly ICompanyClientRepository _companyClientRepository;
    public ClientService(
        IClientRepository clientRepository,
        IClientTypeRepository clientTypeRepository,
         IIndividualClientRepository individualClientRepository, 
    ICompanyClientRepository companyClientRepository,
        IUnitOfWork unitOfWork,
        ILogger<ClientService> logger)
    {
        _clientRepository = clientRepository;
        _clientTypeRepository = clientTypeRepository;
        _individualClientRepository = individualClientRepository; 
        _companyClientRepository = companyClientRepository; 
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ClientDto>> CreateClientAsync(CreateClientDto createDto)
    {
        try
        {
            
            // Валидация уникальности телефона
            if (await _clientRepository.PhoneExistsAsync(createDto.Phone))
            {
                return Result<ClientDto>.Failure("Клиент с таким телефоном уже существует");
            }

            // Валидация уникальности email
            if (await _clientRepository.EmailExistsAsync(createDto.Email))
            {
                return Result<ClientDto>.Failure("Клиент с таким email уже существует");
            }
           
            
            // Создание клиента
            var client = new Client
            {
                Phone = createDto.Phone,
                Email = createDto.Email,
                ClientTypeId = createDto.ClientTypeId
            };

            await _clientRepository.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Создан новый клиент ID: {ClientId}", client.ClientsId);

            var createdClient = await _clientRepository.GetWithDetailsAsync(client.ClientsId);
            var clientDto = MapToClientDto(createdClient!);
            return Result<ClientDto>.Success(clientDto, "Клиент успешно создан");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании клиента");
            return Result<ClientDto>.Failure("Ошибка при создании клиента");
        }
    }

    public async Task<Result<ClientDto>> GetClientByIdAsync(int id)
    {
        try
        {
            var client = await _clientRepository.GetWithDetailsAsync(id);
            if (client == null)
            {
                return Result<ClientDto>.Failure("Клиент не найден");
            }

            var clientDto = MapToClientDto(client);
            return Result<ClientDto>.Success(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении клиента ID: {ClientId}", id);
            return Result<ClientDto>.Failure("Ошибка при получении клиента");
        }
    }

    public async Task<Result<List<ClientDto>>> GetAllClientsAsync()
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = clients.Select(MapToClientDto).ToList();

            return Result<List<ClientDto>>.Success(clientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка клиентов");
            return Result<List<ClientDto>>.Failure("Ошибка при получении списка клиентов");
        }
    }

    public async Task<Result<ClientDto>> UpdateClientAsync(int id, UpdateClientDto updateDto)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return Result<ClientDto>.Failure("Клиент не найден");
            }

            // Проверка уникальности телефона для других клиентов
            if (await _clientRepository.PhoneExistsForOtherClientAsync(id, updateDto.Phone))
            {
                return Result<ClientDto>.Failure("Телефон уже используется другим клиентом");
            }

            // Проверка уникальности email для других клиентов
            if (await _clientRepository.EmailExistsForOtherClientAsync(id, updateDto.Email))
            {
                return Result<ClientDto>.Failure("Email уже используется другим клиентом");
            }

            // Обновление данных
            client.Phone = updateDto.Phone;
            client.Email = updateDto.Email;

            await _clientRepository.UpdateAsync(client);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Обновлен клиент ID: {ClientId}", id);

            var updatedClient = await _clientRepository.GetWithDetailsAsync(id);
            var clientDto = MapToClientDto(updatedClient!);
            return Result<ClientDto>.Success(clientDto, "Данные клиента обновлены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении клиента ID: {ClientId}", id);
            return Result<ClientDto>.Failure("Ошибка при обновлении клиента");
        }
    }

    public async Task<Result> DeleteClientAsync(int id)
    {
        try
        {
            var client = await _clientRepository.GetWithAllRelationsAsync(id);
            if (client == null)
            {
                return Result.Failure("Клиент не найден");
            }

            // Проверка на наличие связанных заказов
            if (client.Orders?.Any() == true)
            {
                return Result.Failure("Невозможно удалить клиента с существующими заказами");
            }

            await _clientRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Удален клиент ID: {ClientId}", id);

            return Result.Success("Клиент успешно удален");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении клиента ID: {ClientId}", id);
            return Result.Failure("Ошибка при удалении клиента");
        }
    }

    public async Task<Result<IndividualClientDto>> CreateIndividualClientAsync(int clientId, IndividualClientDto individualDto)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
            {
                return Result<IndividualClientDto>.Failure("Клиент не найден");
            }

            if (client.ClientTypeId != 1)
            {
                return Result<IndividualClientDto>.Failure("Неверный тип клиента для физического лица");
            }

            // Проверка уникальности паспорта
            if (await _individualClientRepository.PassportNumberExistsAsync(individualDto.PassportNumber))
            {
                return Result<IndividualClientDto>.Failure("Клиент с таким номером паспорта уже существует");
            }

            // Проверяем, не существует ли уже IndividualClient для этого клиента
            var existingIndividual = await _individualClientRepository.GetByClientIdAsync(clientId);
            if (existingIndividual != null)
            {
                return Result<IndividualClientDto>.Failure("Для этого клиента уже созданы данные физ. лица");
            }

            var individualClient = new IndividualClient
            {
                ClientsId = clientId,
                FirstName = individualDto.FirstName,
                LastName = individualDto.LastName,
                PatronymicName = individualDto.PatronymicName,
                PassportNumber = individualDto.PassportNumber,
                PassportDateOfIssue = individualDto.PassportDateOfIssue.HasValue
                    ? DateOnly.FromDateTime(individualDto.PassportDateOfIssue.Value)
                    : null
            };

            // ИСПРАВЛЕНО: используем репозиторий вместо прямого доступа к контексту
            await _individualClientRepository.AddAsync(individualClient);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Создано физ. лицо для клиента ID: {ClientId}", clientId);

            // Возвращаем DTO с ID созданной записи
            var resultDto = new IndividualClientDto
            {
                IndividualClientId = individualClient.IndividualId,
                FirstName = individualClient.FirstName,
                LastName = individualClient.LastName,
                PatronymicName = individualClient.PatronymicName,
                PassportNumber = individualClient.PassportNumber,
                PassportDateOfIssue = individualClient.PassportDateOfIssue.HasValue
                    ? individualClient.PassportDateOfIssue.Value.ToDateTime(new TimeOnly(0, 0))
                    : null
            };

            return Result<IndividualClientDto>.Success(resultDto, "Данные физ. лица сохранены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании физ. лица для клиента ID: {ClientId}", clientId);
            return Result<IndividualClientDto>.Failure("Ошибка при создании физ. лица");
        }
    }

    public async Task<Result<CompanyClientDto>> CreateCompanyClientAsync(int clientId, CompanyClientDto companyDto)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
            {
                return Result<CompanyClientDto>.Failure("Клиент не найден");
            }

            if (client.ClientTypeId != 2)
            {
                return Result<CompanyClientDto>.Failure("Неверный тип клиента для компании");
            }

            // Проверка уникальности ИНН
            if (await _companyClientRepository.InnExistsAsync(companyDto.Inn))
            {
                return Result<CompanyClientDto>.Failure("Компания с таким ИНН уже существует");
            }

            // Проверяем, не существует ли уже CompanyClient для этого клиента
            var existingCompany = await _companyClientRepository.GetByClientIdAsync(clientId);
            if (existingCompany != null)
            {
                return Result<CompanyClientDto>.Failure("Для этого клиента уже созданы данные компании");
            }

            var companyClient = new CompanyClient
            {
                ClientsId = clientId,
                CompanyName = companyDto.CompanyName,
                Inn = companyDto.Inn
            };

            // ИСПРАВЛЕНО: используем репозиторий вместо прямого доступа к контексту
            await _companyClientRepository.AddAsync(companyClient);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Создана компания для клиента ID: {ClientId}", clientId);

            // Возвращаем DTO с ID созданной записи
            var resultDto = new CompanyClientDto
            {
                CompanyClientId = companyClient.CompanyId,
                CompanyName = companyClient.CompanyName,
                Inn = companyClient.Inn
            };

            return Result<CompanyClientDto>.Success(resultDto, "Данные компании сохранены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компании для клиента ID: {ClientId}", clientId);
            return Result<CompanyClientDto>.Failure("Ошибка при создании компании");
        }
    }

    public async Task<Result<List<ClientTypeDto>>> GetClientTypesAsync()
    {
        try
        {
            var clientTypes = await _clientTypeRepository.GetAllAsync();
            var clientTypeDtos = clientTypes.Select(ct => new ClientTypeDto
            {
                ClientTypeId = ct.ClientTypeId,
                TypeName = ct.TypeName
            }).ToList();

            return Result<List<ClientTypeDto>>.Success(clientTypeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении типов клиентов");
            return Result<List<ClientTypeDto>>.Failure("Ошибка при получении типов клиентов");
        }
    }

    // Маппинг сущности в DTO
    private ClientDto MapToClientDto(Client client)
    {
        var clientDto = new ClientDto
        {
            ClientsId = client.ClientsId,
            Phone = client.Phone,
            Email = client.Email,
            ClientTypeId = client.ClientTypeId,
            ClientTypeName = client.ClientType?.TypeName ?? string.Empty,
            UserId = client.UserId
        };

        // Маппинг IndividualClient если есть
        var individualClient = client.IndividualClients.FirstOrDefault();
        if (individualClient != null)
        {
            clientDto.IndividualClient = new IndividualClientDto
            {
                IndividualClientId = individualClient.IndividualId,
                FirstName = individualClient.FirstName,
                LastName = individualClient.LastName,
                PatronymicName = individualClient.PatronymicName,
                PassportNumber = individualClient.PassportNumber
            };
        }

        // Маппинг CompanyClient если есть
        var companyClient = client.CompanyClients.FirstOrDefault();
        if (companyClient != null)
        {
            clientDto.CompanyClient = new CompanyClientDto
            {
                CompanyClientId = companyClient.CompanyId,
                CompanyName = companyClient.CompanyName,
                Inn = companyClient.Inn
            };
        }

        return clientDto;
    }

    public async Task<Result<IndividualClientDto>> UpdateIndividualClientAsync(int clientId, UpdateIndividualClientDto updateDto)
    {
        try
        {
            var ind = await _individualClientRepository.GetByClientIdAsync(clientId);
            if (ind == null)
            {
                return Result<IndividualClientDto>.Failure("Данные физ. лица не найдены");
            }
            if( await _individualClientRepository.PassportNumberExistsForOtherClientAsync(clientId, updateDto.PassportNumber))
            {
                return Result<IndividualClientDto>.Failure("Номер паспорта уже используется другим физ. лицом");
            }

            ind.FirstName = updateDto.FirstName;
            ind.LastName = updateDto.LastName;
            ind.PatronymicName = updateDto.PatronymicName;
            ind.PassportNumber = updateDto.PassportNumber;
            await _individualClientRepository.UpdateAsync(ind);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Обновлено физ. лицо для клиента ID: {ClientId}", clientId);

            var resultDto = new IndividualClientDto
            {
                IndividualClientId = ind.IndividualId,
                FirstName = ind.FirstName,
                LastName = ind.LastName,
                PatronymicName = ind.PatronymicName,
                PassportNumber = ind.PassportNumber,
                PassportDateOfIssue = ind.PassportDateOfIssue.HasValue
                    ? ind.PassportDateOfIssue.Value.ToDateTime(new TimeOnly(0, 0))
                    : null
            };
            return Result<IndividualClientDto>.Success(resultDto, "Данные физ. лица обновлены");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении физ. лица для клиента ID: {ClientId}", clientId);
            return Result<IndividualClientDto>.Failure("Ошибка при обновлении физ. лица");
        }
    }

    public async Task<Result<CompanyClientDto>> UpdateCompanyClientAsync(int clientId, UpdateCompanyClientDto updateDto)
    {
        try
        {
            var comp = await _companyClientRepository.GetByClientIdAsync(clientId);
            if (comp == null)
            {
                return Result<CompanyClientDto>.Failure("Данные компании не найдены");
            }
            if (await _companyClientRepository.InnExistsForOtherCompanyAsync(clientId, updateDto.Inn))
            {
                return Result<CompanyClientDto>.Failure("ИНН уже используется другой компанией");
            }
            comp.CompanyName = updateDto.CompanyName;
            comp.Inn = updateDto.Inn;
            await _companyClientRepository.UpdateAsync(comp);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Обновлена компания для клиента ID: {ClientId}", clientId);
            var resultDto = new CompanyClientDto
            {
                CompanyClientId = comp.CompanyId,
                CompanyName = comp.CompanyName,
                Inn = comp.Inn
            };
            return Result<CompanyClientDto>.Success(resultDto, "Данные компании обновлены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении компании для клиента ID: {ClientId}", clientId);
            return Result<CompanyClientDto>.Failure("Ошибка при обновлении компании");
        }
    }
}