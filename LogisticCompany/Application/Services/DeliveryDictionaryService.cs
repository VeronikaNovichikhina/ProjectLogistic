using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.Application.Services
{
    public class DeliveryDictionaryService : IDeliveryDictionaryService
    {
        private readonly AppDbContext _db;
        public DeliveryDictionaryService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<DeliveryType>> GetDeliveryTypesAsync()
        {
            return await _db.DeliveryTypes.ToListAsync();
        }

        public async Task<List<ParcelTemplate>> GetParcelTemplatesAsync()
        {
            return await _db.ParcelTemplates.ToListAsync();
        }

        public async  Task<List<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _db.PaymentMethods.ToListAsync();
        }

        public async Task<List<TransportType>> GetTransportTypesAsync()
        {
            return await _db.TransportTypes.ToListAsync();
        }

        public async Task<List<DeliveryTariff>> GetDeliveryTariffAsync()
        {
            return await _db.DeliveryTariffs.ToListAsync();
        }

        public async Task<List<StatusDelivery>> GetStatusDeliveryAsync()
        {
            return await _db.StatusDeliveries.ToListAsync();
        }
    }
}
