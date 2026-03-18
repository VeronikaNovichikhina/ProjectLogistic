using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Entities.Tracking;
using LogisticCompany.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.JSInterop;
using static LogisticCompany.Components.Pages.OrderPages.EditOrdersBeforePayment;

namespace LogisticCompany.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IJSRuntime JS;

        public OrderService(AppDbContext db, IJSRuntime jsRuntime)
        {
            _db = db;
            JS = jsRuntime;
        }

        private async Task<string> GetTownName(int townId)
        {
            var town = await _db.Towns.FirstOrDefaultAsync(t => t.TownId == townId);
            return town?.TownName ?? "Неизвестно";
        }
        public async Task<CreateOrderResult> CreateOrderAsync(CreateOrderRequest r, string? newClientEmail = null)
        {
            if (r.OriginTownId <= 0)
                throw new Exception("Не выбран город отправления");

            if (r.DestinationTownId <= 0)
                throw new Exception("Не выбран город назначения");

            if (r.PickupBranchId <= 0)
                throw new Exception("Не выбран пункт отправления");

            if (r.DeliveryTypeId == 2 && r.DestinationBranchId <= 0)
                throw new Exception("Не выбран пункт назначения");

            if (r.DeliveryTypeId == 1 && string.IsNullOrWhiteSpace(r.CourierAddress))
                throw new Exception("Не указан адрес доставки");
            
           
            await using var transaction = await _db.Database.BeginTransactionAsync();


            try
            {
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientsId == r.ClientId);
                if (client == null) throw new Exception("Клиент не найден");
                string? generatedPassword = null;
                bool isNewUserCreated = false;
                User? user = null;
                if (!string.IsNullOrWhiteSpace(newClientEmail))
                {
                    var normalizedEmail = newClientEmail.ToLower();
                    user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

                    if (user == null)
                    {
                        generatedPassword = GenerateSecurePassword();

                        user = new User
                        {
                            Email = normalizedEmail,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
                            Role = "User",
                            IsTemporaryPassword = true
                        };

                        _db.Users.Add(user);
                        await _db.SaveChangesAsync();

                        isNewUserCreated = true;
                    }
                    if (client.UserId != user.Id)
                    {
                        client.UserId = user.Id;
                        _db.Clients.Update(client);
                        await _db.SaveChangesAsync();
                    }
                }

            
                var order = new Order
                {
                    ClientsId = r.ClientId,
                    CourierDestAddress = r.CourierAddress,
                    FirstRecepientName = r.FirstName,
                    MiddleRecepientName = r.MiddleName,
                    LastRecepientName = r.LastName,
                    PhoneRecepient = r.Phone,
                    TemplateId = r.ParcelTemplateId,
                    DescriptionParcel = r.Description,
                    OriginTownId = r.OriginTownId,
                    DestinationTownId = r.DestinationTownId,
                    PickupBranchesId = r.PickupBranchId,
                    DestinationBranchId = r.DestinationBranchId,
                    DeliveryTypeId = r.DeliveryTypeId,
                    TransportTypeId = r.TransportTypeId,
                    LengthCm = r.LengthCm,
                    WidthCm = r.WidthCm,
                    HeightCm = r.HeightCm,
                    Weight = r.Weight

                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();
                order.OrderNumber = $"ORD-{DateTime.Now:yyMMdd}-{order.OrdersId:D6}";
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();

                if (r.PaymentMethodId <= 0)
                {
                    throw new Exception("Выберите способ оплаты");
                }

                var payment = new Payment
                {
                    Orders = order,
                    PaymentMethodId = r.PaymentMethodId,
                    Amount = r.Amount,
                    PaymentDate = DateTime.Now
                };
                _db.Payments.Add(payment);

                var tracking = new Tracking
                {
                    OrdersId = order.OrdersId,
                    LocationTrackings = await GetTownName(order.OriginTownId),
                    UpdateDate = DateTime.Now,
                    StatusId = 2, 
                    BranchesId = order.PickupBranchesId
                };
                _db.Trackings.Add(tracking);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return new CreateOrderResult
                {
                    OrderId = order.OrdersId,
                    Email = isNewUserCreated ? user!.Email : null,
                    TemporaryPassword = isNewUserCreated ? generatedPassword : null
                };

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

     

        private string GenerateSecurePassword(int length = 8)
        {

            var random = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }



        public async Task<int> CreateOrderByUserAsync(CreateOrderRequest r)
        {
            if (r.OriginTownId <= 0)
                throw new Exception("Не выбран город отправления");

            if (r.DestinationTownId <= 0)
                throw new Exception("Не выбран город назначения");

            if (r.PickupBranchId <= 0)
                throw new Exception("Не выбран пункт отправления");

            if (r.DeliveryTypeId == 2 && r.DestinationBranchId <= 0)
                throw new Exception("Не выбран пункт назначения");

            if (r.DeliveryTypeId == 1 && string.IsNullOrWhiteSpace(r.CourierAddress))
                throw new Exception("Не указан адрес доставки");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var client = await _db.Clients
                    .FirstOrDefaultAsync(c => c.ClientsId == r.ClientId);

                if (client == null)
                    throw new Exception("Клиент не найден");

                var order = new Order
                {
                    ClientsId = r.ClientId,
                    CourierDestAddress = r.CourierAddress,
                    FirstRecepientName = r.FirstName,
                    MiddleRecepientName = r.MiddleName,
                    LastRecepientName = r.LastName,
                    PhoneRecepient = r.Phone,
                    TemplateId = r.ParcelTemplateId,
                    DescriptionParcel = r.Description,
                    OriginTownId = r.OriginTownId,
                    DestinationTownId = r.DestinationTownId,
                    PickupBranchesId = r.PickupBranchId,
                    DestinationBranchId = r.DestinationBranchId,
                    DeliveryTypeId = r.DeliveryTypeId,
                    TransportTypeId = r.TransportTypeId,
                    LengthCm = r.LengthCm,
                    WidthCm = r.WidthCm,
                    HeightCm = r.HeightCm,
                    Weight = r.Weight,
                    CalculatedPrice = r.CalculatedPrice
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                order.OrderNumber = $"{DateTime.Now:yyyyMMdd}-{order.OrdersId}";
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();

                var tracking = new Tracking
                {
                    OrdersId = order.OrdersId,
                    LocationTrackings = await GetTownName(order.OriginTownId),
                    UpdateDate = DateTime.Now,
                    StatusId = 1, 
                    BranchesId = order.PickupBranchesId
                };

                _db.Trackings.Add(tracking);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return order.OrdersId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<List<Order>> GetOrdersForManagerAsync(int branchId)
        {
            return await _db.Orders
                .Include(o => o.Template)
                .Include(o => o.DeliveryType)
                .Include(o => o.TransportType)
                .Include(o => o.OriginTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.Clients)
                .Where(o => o.PickupBranchesId == branchId || o.DestinationBranchId == branchId)
                .OrderByDescending(o => o.OrdersId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersForClientAsync(int clientId)
        {
            return await _db.Orders
                .Include(o => o.Template)
                .Include(o => o.DeliveryType)
                .Include(o => o.TransportType)
                .Include(o => o.OriginTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown)
                    .ThenInclude(t => t.Country)
                .Where(o => o.ClientsId == clientId)
                .OrderByDescending(o => o.OrdersId)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _db.Orders
                .Include(o => o.Template)
                .Include(o => o.DeliveryType)
                .Include(o => o.TransportType)
                .Include(o => o.OriginTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.Clients)
                .Include(o => o.Trackings)
                    .ThenInclude(t => t.Status)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);
        }

        public async Task<List<Order>> GetOrdersForAdminAsync(int branchId)
        {
            return await _db.Orders
                .Include(o => o.Template)
                .Include(o => o.DeliveryType)
                .Include(o => o.TransportType)
                .Include(o => o.OriginTown).ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown).ThenInclude(t => t.Country)
                .Include(o => o.Clients)
                .Where(o => o.PickupBranchesId == branchId || o.DestinationBranchId == branchId)
                .OrderByDescending(o => o.OrdersId)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId)
        {
            return await _db.Orders
        .Include(o => o.DeliveryType)
        .Include(o => o.TransportType)
        .Include(o => o.OriginTown)
            .ThenInclude(t => t.Country)
        .Include(o => o.DestinationTown)
            .ThenInclude(t => t.Country)
        .Include(o => o.PickupBranches)
        .Include(o => o.Clients)
        .Include(o => o.Trackings)
            .ThenInclude(t => t.Status)
        .FirstOrDefaultAsync(o => o.OrdersId == orderId);
        }

        public async Task<List<StatusDelivery>> GetStatusDeliveriesAsync()
        {
            return await _db.StatusDeliveries.AsNoTracking().ToListAsync();
        }

        public async Task<Order> GetOrderForPaymentAsync(int orderId)
        {
            return await   _db.Orders
                .Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .Include(o => o.Trackings)
                    .ThenInclude(t => t.Status)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);

        }
        public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _db.PaymentMethods.ToListAsync();
        }

        public async Task<OrderEditModel?> GetOrderForEditAsync(int orderId)
        {
            
            var order = await _db.Orders
                .Include(o => o.OriginTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown)
                    .ThenInclude(t => t.Country)
                .Include(o => o.DeliveryType)
                .Include(o => o.Template)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);

            if (order == null)
                return null;

            return new OrderEditModel
            {
                OrdersId = order.OrdersId,
                LastRecepientName = order.LastRecepientName,
                FirstRecepientName = order.FirstRecepientName,
                MiddleRecepientName = order.MiddleRecepientName,
                PhoneRecepient = order.PhoneRecepient,
                DescriptionParcel = order.DescriptionParcel,
                LengthCm = order.LengthCm,
                WidthCm = order.WidthCm,
                HeightCm = order.HeightCm,
                Weight = order.Weight,
                CourierDestAddress = order.CourierDestAddress,
            };
        }

        public async Task<bool> SaveOrderAsync(int orderId, OrderEditModel editModel)
        {
           

            var orderToUpdate = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);

            if (orderToUpdate == null)
                return false;

            // Обновляем данные заказа
            orderToUpdate.LastRecepientName = editModel.LastRecepientName;
            orderToUpdate.FirstRecepientName = editModel.FirstRecepientName;
            orderToUpdate.MiddleRecepientName = editModel.MiddleRecepientName;
            orderToUpdate.PhoneRecepient = editModel.PhoneRecepient;
            orderToUpdate.DescriptionParcel = editModel.DescriptionParcel;

            if (orderToUpdate.TemplateId == null)
            {
                orderToUpdate.LengthCm = editModel.LengthCm;
                orderToUpdate.WidthCm = editModel.WidthCm;
                orderToUpdate.HeightCm = editModel.HeightCm;
                orderToUpdate.Weight = editModel.Weight;
            }

            if (orderToUpdate.DeliveryTypeId == 1)
            {
                orderToUpdate.CourierDestAddress = editModel.CourierDestAddress;
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
