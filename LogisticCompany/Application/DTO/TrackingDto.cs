namespace LogisticCompany.Application.DTO
{
    public class TrackingDto
    {
        public int TrackingId { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Branch { get; set; }
    }

    public class OrderSummaryDto
    {
        public int OrdersId { get; set; }
        public string? OrderNumber { get; set; }
        public string? FirstRecepientName { get; set; }
        public string? LastRecepientName { get; set; }
        public string? PhoneRecepient { get; set; }
        public string? DescriptionParcel { get; set; }
    }

    public class OrderDetailsDto
    {
        public int OrdersId { get; set; }
        public string? OrderNumber { get; set; }
        public string? FirstRecepientName { get; set; }
        public string? LastRecepientName { get; set; }
        public string? MiddleRecepientName { get; set; }
        public string? PhoneRecepient { get; set; }
        public string? DescriptionParcel { get; set; }
        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? Weight { get; set; }
        public string? CourierDestAddress { get; set; }
        public string? OriginTown { get; set; }
        public string? OriginCountry { get; set; }
        public string? DestinationTown { get; set; }
        public string? DestinationCountry { get; set; }
        public string? DeliveryType { get; set; }
        public string? TransportType { get; set; }
        public string? PickupBranch { get; set; }
        public string? CurrentStatus { get; set; }
        public string? LastLocation { get; set; }
        public string? Amount { get; set; }
    }
}
