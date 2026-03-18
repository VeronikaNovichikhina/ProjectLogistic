namespace LogisticCompany.Application.DTO
{
    public class CreateOrderResult
    {
        public int OrderId { get; set; }
        public string Email { get; set; } = "";
        public string? TemporaryPassword { get; set; }
    }
}
