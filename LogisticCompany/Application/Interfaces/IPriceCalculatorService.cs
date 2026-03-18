using LogisticCompany.Application.DTO;
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.DTO;

namespace LogisticCompany.Application.Interfaces
{
    public interface IPriceCalculatorService
    {
        Task<PriceCalculationResult> CalculateAsync(
        OrdersDTO order,
        List<Town> towns,
        List<DeliveryTariff> deliveryTariffs,
        List<ParcelTemplate> parcelTemplates,
        int selectedTypeParcel);
    }
}
