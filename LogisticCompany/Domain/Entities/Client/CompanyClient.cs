using System;
using System.Collections.Generic;

public partial class CompanyClient
{
    public int CompanyId { get; set; }

    public int ClientsId { get; set; }

    public string Inn { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public virtual Client Clients { get; set; } = null!;
}
