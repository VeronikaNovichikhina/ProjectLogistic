namespace LogisticCompany.Domain.Entities.Orders
{
    public partial class ParcelTemplate
    {
        public int TemplateId { get; set; }

        public string TemplateName { get; set; } = null!;

        public int? LengthCm { get; set; }

        public int? WidthCm { get; set; }

        public int? HeightCm { get; set; }

        public int? MaxWeight { get; set; }


        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
