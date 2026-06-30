using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly AppDbContext _db; 

        public LocationService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Branch>> GetAllBranchesAsync()
        {
            return await _db.Branches.ToListAsync();
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<List<Town>> GetAllTownAsync()
        {
            return await _db.Towns.ToListAsync();
        }

        public  IEnumerable<Town> GetTownsByCountry(int countryId, IEnumerable<Town> towns) =>
       towns.Where(t => t.CountryId == countryId);

        public  IEnumerable<Branch> GetBranchesByTown(int townId, IEnumerable<Branch> branches) =>
            branches.Where(b => b.TownId == townId);

        public void UpdateLocationTracking(OrdersDTO order, int originTownId, IEnumerable<Town> towns)
        {
            order.OriginTownId = originTownId;
            var town = towns.FirstOrDefault(t => t.TownId == originTownId);
            order.LocationTracking = town?.TownName ?? "Неизвестно";
            order.TrackingBranchId = order.PickupBranchesId;
        }

        
    }
}
