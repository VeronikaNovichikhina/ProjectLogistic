using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;

namespace LogisticCompany.Application.DTO
{
    public class PriceCalculationRequest
    {
        public int OriginTownId { get; set; }
        public int DestinationTownId { get; set; }
        public int TransportTypeId { get; set; }
        public int DeliveryTypeId { get; set; }
        public int ParcelType { get; set; } 
        public int? ParcelTemplateId { get; set; }
        public decimal? CustomWeight { get; set; }
        public List<Town> Towns { get; set; }
        public List<DeliveryTariff> DeliveryTariffs { get; set; }
        public List<ParcelTemplate> ParcelTemplates { get; set; }
    }
}
