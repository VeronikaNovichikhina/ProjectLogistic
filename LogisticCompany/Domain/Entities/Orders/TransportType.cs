using System;
using System.Collections.Generic;

namespace LogisticCompany.Domain.Entities.Orders;

public partial class TransportType
{
    public int TransportTypeId { get; set; }

    public string NameTransportType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
