
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using System;
using System.Collections.Generic;

namespace LogisticCompany.Domain.Entities.Tracking;

public partial class Tracking
{
    public int TrackingsId { get; set; }

    public int OrdersId { get; set; }

    public string LocationTrackings { get; set; } = null!;

    public DateTime UpdateDate { get; set; }

    public int StatusId { get; set; }

    public int? BranchesId { get; set; }

    public virtual Branch? Branches { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual StatusDelivery Status { get; set; } = null!;
}
