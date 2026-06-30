namespace LogisticCompany.Application.model
{
    public class OrderEditModel
    {
        public int OrdersId { get; set; }

        // получатель
        public string LastRecepientName { get; set; } = string.Empty;
        public string FirstRecepientName { get; set; } = string.Empty;
        public string? MiddleRecepientName { get; set; }
        public string PhoneRecepient { get; set; } = string.Empty;

        // посылка
        public string DescriptionParcel { get; set; } = string.Empty;
        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? Weight { get; set; }
        public string? CourierDestAddress { get; set; }

        // 🔥 ВАЖНО: ID (для списков)
        public int? OriginTownId { get; set; }
        public int? DestinationTownId { get; set; }
        public int? DeliveryTypeId { get; set; }
        public int? TemplateId { get; set; }

        // 🔥 ДЛЯ ОТОБРАЖЕНИЯ
        public string? OriginTownName { get; set; }
        public string? OriginCountryName { get; set; }

        public string? DestinationTownName { get; set; }
        public string? DestinationCountryName { get; set; }

        public string? DeliveryTypeName { get; set; }

        public string TotalAmount { get; set; }
    }
}
