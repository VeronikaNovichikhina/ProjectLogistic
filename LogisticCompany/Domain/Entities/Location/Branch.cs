using LogisticCompany.Domain.Entities.Employee;
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Entities.Tracking;
using System;
using System.Collections.Generic;


public partial class Branch
{
    public int BranchesId { get; set; }

    public string NameBranches { get; set; } = null!;

    public string AddressBranches { get; set; } = null!;

    public string PhoneBranches { get; set; } = null!;

    public int? TownId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Town? Town { get; set; }

    public virtual ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
