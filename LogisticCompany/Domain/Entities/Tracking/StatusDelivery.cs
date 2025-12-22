using LogisticCompany.Domain.Entities.Tracking;
using System;
using System.Collections.Generic;


public partial class StatusDelivery
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();
}
