namespace LogisticCompany.DTO
{
    public class OrderEditModel
    {
        public int OrdersId { get; set; }
        public string LastRecepientName { get; set; } = string.Empty;
        public string FirstRecepientName { get; set; } = string.Empty;
        public string? MiddleRecepientName { get; set; }
        public string PhoneRecepient { get; set; } = string.Empty;
        public string DescriptionParcel { get; set; } = string.Empty;
        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? Weight { get; set; }
        public string? PickupAddress { get; set; }
        public string? CourierDestAddress { get; set; }
        public string? CountryDestAddress { get; set; }
        public string? TownDestAddress { get; set; }
        public string? TownOriginAddress { get; set; }
        public string? CountryOriginAddress { get; set; }
    }
}
