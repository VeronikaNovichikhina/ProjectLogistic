using LogisticCompany.Domain.Entities.Orders;

public partial class Client
{
    public int ClientsId { get; set; }
    public int? UserId { get; set; } // Новая связь
    public int ClientTypeId { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual User User { get; set; }
    public virtual ClientType ClientType { get; set; } = null!;

    public virtual ICollection<CompanyClient> CompanyClients { get; set; } = new List<CompanyClient>();

    public virtual ICollection<IndividualClient> IndividualClients { get; set; } = new List<IndividualClient>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
