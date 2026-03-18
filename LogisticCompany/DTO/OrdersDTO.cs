namespace LogisticCompany.DTO
{
    public class OrdersDTO
    {
        public string OrderNumber { get; set; } = string.Empty;
        public int StatusId { get; set; } = 1; 
        public string? LocationTracking { get; set; }
        public int? TrackingBranchId { get; set; }
        public int ClientsId { get; set; }
        public string? CourierDestAddress { get; set; }

        public string FirstRecepientName { get; set; } = string.Empty;
        public string? MiddleRecepientName { get; set; }
        public string LastRecepientName { get; set; } = string.Empty;
        public string PhoneRecepient { get; set; } = string.Empty;

        
        public int ParcelTemplateId { get; set; }
        public decimal? LengthCm { get; set; }

        public decimal? WidthCm { get; set; }

        public decimal? HeightCm { get; set; }

        public decimal? Weight { get; set; }

        public string DescriptionParcel { get; set; } = string.Empty;

        public int OriginTownId { get; set; }
        public int OriginCountryId { get; set; }

        public int DestinationCountryId { get; set; }
        public int DestinationTownId { get; set; }

        public int PickupBranchesId { get; set; }
        public int DestinationBranchesId { get; set; }

        public int DeliveryTypeId { get; set; }
        public int TransportTypeId { get; set; }

        public int PaymentMethodId { get; set; }

        public string Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public decimal CalculatedPrice { get; set; }
        public decimal Distance { get; set; }

    }

}
