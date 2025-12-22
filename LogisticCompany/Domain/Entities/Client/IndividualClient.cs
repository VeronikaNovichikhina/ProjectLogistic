using System;
using System.Collections.Generic;


public partial class IndividualClient
{
    public int IndividualId { get; set; }

    public int ClientsId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? PatronymicName { get; set; }

    public string LastName { get; set; } = null!;

    public string PassportNumber { get; set; } = null!;

    public DateOnly? PassportDateOfIssue { get; set; }

    public virtual Client Clients { get; set; } = null!;
}
