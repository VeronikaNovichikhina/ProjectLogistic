namespace LogisticCompany.Domain.Repositories
{
    public interface IClientTypeRepository
    {
        Task<List<ClientType>> GetAllAsync();
        Task<ClientType?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<ClientType?> GetByNameAsync(string typeName);
    }
}
