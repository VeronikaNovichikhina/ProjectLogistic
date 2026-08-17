using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using Microsoft.EntityFrameworkCore;
using static LogisticCompany.Components.Pages.Home;

namespace LogisticCompany.Application.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly AppDbContext _context;

        public TrackingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrackingDto>> GetOrderTrackingsAsync(int orderId)
        {
            return await _context.Trackings
                .Include(t => t.Status)
                .Include(t => t.Branches)
                .Where(t => t.OrdersId == orderId)
                .OrderByDescending(t => t.UpdateDate)
                .Select(t => new TrackingDto
                {
                    TrackingId = t.TrackingsId,
                    Status = t.Status != null ? t.Status.StatusName : null,
                    Location = t.LocationTrackings,
                    UpdateDate = t.UpdateDate,
                    Branch = t.Branches != null ? t.Branches.NameBranches : null
                })
                .ToListAsync();
        }

        public async Task<TrackingSearchResult?> SearchByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new Exception("Номер заказа не указан");

            var order = await _context.Orders
                .Include(o => o.Trackings)
                    .ThenInclude(t => t.Status)
                .Include(o => o.Trackings)
                    .ThenInclude(t => t.Branches)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return null;

            return new TrackingSearchResult
            {
                Order = order,
                TrackingHistory = order.Trackings
                    .OrderByDescending(t => t.UpdateDate)
                    .ToList()
            };
        }
    }

}
