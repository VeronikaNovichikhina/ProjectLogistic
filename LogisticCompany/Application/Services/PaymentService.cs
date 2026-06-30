using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public PaymentService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task ProcessPaymentAsync(int orderId, int paymentMethodId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var order = await db.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);

            if (order == null)
                throw new Exception("Заказ не найден");

            if (order.Payments.Any(p => p.PaymentDate != null))
                throw new Exception("Заказ уже оплачен");

            var payment = order.Payments.FirstOrDefault();

            if (payment == null)
            {
                payment = new Payment
                {
                    OrdersId = orderId,
                    Amount = order.CalculatedPrice?.ToString("F2") ?? "0.00",
                    PaymentMethodId = paymentMethodId,
                    PaymentDate = DateTime.Now
                };

                db.Payments.Add(payment);
            }
            else
            {
                payment.PaymentMethodId = paymentMethodId;
                payment.PaymentDate = DateTime.Now;
            }

            db.Trackings.Add(new Tracking
            {
                OrdersId = orderId,
                StatusId = 2, 
                UpdateDate = DateTime.Now,
                LocationTrackings = "Оплата подтверждена",
                BranchesId = order.PickupBranchesId
            });

            await db.SaveChangesAsync();
        }
    }

}
