using System;
using System.Collections.Generic;


public partial class ClientType
{
    public int ClientTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
