namespace LogisticCompany.Application.Interfaces
{
    public interface IPaymentService
    {
        Task ProcessPaymentAsync(int orderId, int paymentMethodId);
    }
}
