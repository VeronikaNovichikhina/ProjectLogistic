namespace LogisticCompany.Application.Interfaces
{
    public interface IOrderQueryService
    {
        Task<List<Order>> GetOrdersWithDetailsAsync();
        Task<List<StatusDelivery>> GetStatusesAsync();
        Task<Order?> GetOrderByIdWithDetailsAsync(int orderId);
    }
}
