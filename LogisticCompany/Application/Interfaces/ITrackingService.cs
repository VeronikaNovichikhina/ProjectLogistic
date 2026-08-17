using LogisticCompany.Application.DTO;
using static LogisticCompany.Components.Pages.Home;

namespace LogisticCompany.Application.Interfaces
{
    public interface ITrackingService
    {
        Task<TrackingSearchResult?> SearchByOrderNumberAsync(string orderNumber);
        Task<List<TrackingDto>> GetOrderTrackingsAsync(int orderId);
    }
}
