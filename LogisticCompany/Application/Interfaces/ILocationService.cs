using LogisticCompany.Application.DTO;
using LogisticCompany.Domain.Entities.Location;

namespace LogisticCompany.Application.Interfaces
{
    public interface ILocationService
    {
        Task<List<Town>> GetAllTownAsync();
        Task<List<Country>> GetAllCountriesAsync();
        Task<List<Branch>> GetAllBranchesAsync();

        IEnumerable<Town> GetTownsByCountry(int countryId, IEnumerable<Town> towns);
        IEnumerable<Branch> GetBranchesByTown(int townId, IEnumerable<Branch> branches);

        void UpdateLocationTracking(OrdersDTO order, int townId, IEnumerable<Town> towns);
    }
}
