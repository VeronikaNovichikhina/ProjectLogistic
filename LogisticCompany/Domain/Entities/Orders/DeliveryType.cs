using LogisticCompany.Domain.Entities.Orders;
using System;
using System.Collections.Generic;


public partial class DeliveryType
{
    public int DeliveryTypeId { get; set; }

    public string NameDeliveryType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
