using LogisticCompany.Domain.Entities.Orders;

namespace LogisticCompany.Application.Interfaces
{
    public interface IDeliveryDictionaryService
    {
        Task<List<DeliveryType>> GetDeliveryTypesAsync();
        Task<List<TransportType>> GetTransportTypesAsync();
        Task<List<PaymentMethod>> GetPaymentMethodsAsync();
        Task<List<ParcelTemplate>> GetParcelTemplatesAsync();

        Task<List<DeliveryTariff>> GetDeliveryTariffAsync();

        Task<List<StatusDelivery>> GetStatusDeliveryAsync();
    }
}
