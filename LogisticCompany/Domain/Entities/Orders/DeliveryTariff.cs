namespace LogisticCompany.Domain.Entities.Orders
{
    public class DeliveryTariff
    {
        public int TariffId { get; set; }
        public int DeliveryTypeId { get; set; }
        public int TransportTypeId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal PricePerKg { get; set; }
        public bool IsActive { get; set; }
    }
}
