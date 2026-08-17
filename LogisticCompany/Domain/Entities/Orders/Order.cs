
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Entities.Tracking;
using System;
using System.Collections.Generic;


public partial class Order
{
    public int OrdersId { get; set; }
    public string? OrderNumber { get; set; } 

    public int ClientsId { get; set; }

    public string? CourierDestAddress { get; set; }

    public string FirstRecepientName { get; set; } = null!;

    public string? MiddleRecepientName { get; set; }

    public string LastRecepientName { get; set; } = null!;

    public string PhoneRecepient { get; set; } = null!;

    public string DescriptionParcel { get; set; } = null!;

    public int OriginTownId { get; set; }

    public int DestinationTownId { get; set; }

    public int PickupBranchesId { get; set; }

    public int? DeliveryTypeId { get; set; }

    public int TransportTypeId { get; set; }

    public int? DestinationBranchId { get; set; }

    public int? TemplateId { get; set; }

    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? Weight { get; set; }
    public decimal? CalculatedPrice { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public virtual Client Clients { get; set; } = null!;

    public virtual DeliveryType? DeliveryType { get; set; }

    public virtual Town DestinationTown { get; set; } = null!;

    public virtual Town OriginTown { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Branch PickupBranches { get; set; } = null!;
    public virtual Branch? DestinationBranch { get; set; }

    public virtual ParcelTemplate? Template { get; set; }

    public virtual ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();

    public virtual TransportType TransportType { get; set; } = null!;
    
}
