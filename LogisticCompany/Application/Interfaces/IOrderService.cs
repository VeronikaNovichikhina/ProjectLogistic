using LogisticCompany.Application.DTO;
using LogisticCompany.Application.model;
using LogisticCompany.Domain.Entities.Orders;

namespace LogisticCompany.Application.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResult> CreateOrderAsync(CreateOrderRequest request, string? newClientEmail = null);
        Task<int> CreateOrderByUserAsync(CreateOrderRequest request);

        Task<List<Order>> GetOrdersForManagerAsync(int branchId);
        Task<List<Order>> GetOrdersForClientAsync(int clientId);
        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<Order>> GetOrdersForAdminAsync(int branchId);
        Task<Order?> GetOrderDetailsAsync(int orderId);

        Task<List<StatusDelivery>> GetStatusDeliveriesAsync();

        Task<Order> GetOrderForPaymentAsync(int orderId);

        Task<List<PaymentMethod>> GetPaymentMethodsAsync();

        Task<OrderEditModel?> GetOrderForEditAsync(int orderId);

        Task<bool> SaveOrderAsync(int orderId, OrderEditModel editModel);
    }
}
