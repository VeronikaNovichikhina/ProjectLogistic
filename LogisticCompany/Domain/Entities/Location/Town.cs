using LogisticCompany.Domain.Entities.Orders;
using System;
using System.Collections.Generic;

namespace LogisticCompany.Domain.Entities.Location;

public partial class Town
{
    public int TownId { get; set; }

    public int CountryId { get; set; }

    public string TownName { get; set; } = null!;

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Order> OrderDestinationTowns { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderOriginTowns { get; set; } = new List<Order>();
}
