using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Application.Services
{
    public class OrderQueryService : IOrderQueryService
    {
        private readonly AppDbContext _context;

        public OrderQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.Clients)
                    .ThenInclude(c => c.IndividualClients)
                .Include(o => o.Clients)
                    .ThenInclude(c => c.CompanyClients)
                .Include(o => o.OriginTown)
                .Include(o => o.DestinationTown)
                .Include(o => o.DeliveryType)
                .Include(o => o.Template)
                .Include(o => o.Trackings)
                    .ThenInclude(t => t.Status)
                 .OrderByDescending(o => o.OrdersId)
                .ToListAsync();
        }

        public async Task<List<StatusDelivery>> GetStatusesAsync()
        {
            return await _context.StatusDeliveries.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Clients)
                    .ThenInclude(c => c.IndividualClients)
                .Include(o => o.Clients)
                    .ThenInclude(c => c.CompanyClients)
                .Include(o => o.OriginTown)
                .Include(o => o.DestinationTown)
                .Include(o => o.DeliveryType)
                .Include(o => o.TransportType)
                .Include(o => o.Template)
                .Include(o => o.PickupBranches)
                .Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);
        }

    }

}
