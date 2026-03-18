namespace LogisticCompany.Application.Interfaces
{
    public interface IMapService
    {
        Task<decimal> GetDistanceAsync(
            string fromCity, string toCity, int isAirTransport);
    }
}
