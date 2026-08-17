using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;

namespace LogisticCompany.Application.Services
{
    public class PriceCalculationService : IPriceCalculatorService
    {
        private readonly IMapService _mapService;

        public PriceCalculationService(IMapService mapService)
        {
            _mapService = mapService;
        }

        public async Task<PriceCalculationResult> CalculateAsync(
            OrdersDTO order,
            List<Town> towns,
            List<DeliveryTariff> deliveryTariffs,
            List<ParcelTemplate> parcelTemplates,
            int selectedTypeParcel)
        {
            var result = new PriceCalculationResult();

            var tariff = GetTariff(
                order.DeliveryTypeId,
                order.TransportTypeId,
                deliveryTariffs);

            if (tariff == null)
                return result;

            var weight = GetWeight(order, parcelTemplates, selectedTypeParcel);

            // Курьерская доставка — без расстояния
            if (order.DeliveryTypeId == 1)
            {
                result.Price = tariff.BasePrice + (weight * tariff.PricePerKg);
                result.Distance = 0;
                return result;
            }

            // Доставка между филиалами — считаем расстояние
            if (order.OriginTownId == 0 || order.DestinationTownId == 0)
                return result;

            var distance = await _mapService.GetDistanceAsync(
                order.OriginTownId,
                order.DestinationTownId,
                order.TransportTypeId);

            result.Price =
                tariff.BasePrice +
                (distance * tariff.PricePerKm) +
                (weight * tariff.PricePerKg);

            result.Distance = distance;

            return result;
        }

        private DeliveryTariff? GetTariff(
            int? deliveryTypeId,
            int transportTypeId,
            List<DeliveryTariff> deliveryTariffs)
        {
            if (deliveryTypeId == null)
                return null;

            return deliveryTariffs.FirstOrDefault(t =>
                t.IsActive &&
                t.DeliveryTypeId == deliveryTypeId &&
                t.TransportTypeId == transportTypeId);
        }

        private decimal GetWeight(
            OrdersDTO order,
            List<ParcelTemplate> parcelTemplates,
            int selectedTypeParcel)
        {
            var actualWeight = order.Weight ?? 0;

            if (selectedTypeParcel == 1)
            {
                var template = parcelTemplates
                    .FirstOrDefault(t => t.TemplateId == order.ParcelTemplateId);

                if (template == null)
                    return actualWeight;

                return Math.Max(actualWeight, (decimal)template.MaxWeight);
            }

            return actualWeight;
        }
    }
}