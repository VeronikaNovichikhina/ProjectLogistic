namespace LogisticCompany.Application.DTO
{
    public class CreateOrderRequest
    {
        public int ClientId { get; set; }
        public string? CourierAddress { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int ParcelTemplateId { get; set; }
        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? Weight { get; set; }
        public string Description { get; set; } = string.Empty;

        public int OriginTownId { get; set; }
        public int DestinationTownId { get; set; }
        public int PickupBranchId { get; set; }
        public int DestinationBranchId { get; set; }

        public int DeliveryTypeId { get; set; }
        public int TransportTypeId { get; set; }

        public int PaymentMethodId { get; set; }
        public string Amount { get; set; } = string.Empty;

        public decimal CalculatedPrice { get; set; }

    }

}
