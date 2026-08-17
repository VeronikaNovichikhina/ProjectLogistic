namespace LogisticCompany.Application.Interfaces
{
    public interface IMapService
    {
        Task<decimal> GetDistanceAsync(int originTownId,int destinationTownId,int transportTypeId);

        bool IsAirOnlyRoute(int originCountryId,int destinationCountryId);
    }
}
