namespace LogisticCompany.Domain.Repositories
{
    public interface IOrderRepository: IRepository<Order>

    {
        Task<Order> GetByIdWithDetailsAsyns(int id);
        Task<IEnumerable<Order>> GetByClientIdAsync(int clientId);
        Task<Order> GetByTrackingNumberAsync(string trackingNumber);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
