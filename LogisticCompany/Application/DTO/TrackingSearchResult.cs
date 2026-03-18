using LogisticCompany.Domain.Entities.Tracking;

namespace LogisticCompany.Application.DTO
{
    public class TrackingSearchResult
    {
        public Order Order { get; set; } = null!;
        public List<Tracking> TrackingHistory { get; set; } = new();

        public StatusDelivery? CurrentStatus =>
            TrackingHistory.FirstOrDefault()?.Status;
    }
}
