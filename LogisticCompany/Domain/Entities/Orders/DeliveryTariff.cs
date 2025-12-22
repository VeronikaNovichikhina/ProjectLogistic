namespace LogisticCompany.Domain.Entities.Orders
{
    public class DeliveryTariff
    {
        public int TariffId { get; set; }
        public int DeliveryTypeId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal MinDistance { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public bool IsActive { get; set; }


    }
}
