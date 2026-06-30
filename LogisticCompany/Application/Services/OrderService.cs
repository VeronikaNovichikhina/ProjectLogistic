using LogisticCompany.Application.DTO;
using LogisticCompany.Application.Interfaces;
using LogisticCompany.Application.model;
using LogisticCompany.Db;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Entities.Tracking;
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
        private readonly IPasswordService _passwordService;

        public OrderService(AppDbContext db, IPasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        private async Task<string> GetTownNameAsync(int townId)
        {
            var town = await _db.Towns.FirstOrDefaultAsync(t => t.TownId == townId);
            return town?.TownName ?? "Неизвестно";
        }
        public async Task<CreateOrderResult> CreateOrderAsync(CreateOrderRequest r, string? newClientEmail = null)
        {
            ValidateOrderRequest(r);

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientsId == r.ClientId)
                    ?? throw new Exception("Клиент не найден");

                string? generatedPassword = null;
                bool isNewUserCreated = false;
                User? user = null;

                if (!string.IsNullOrWhiteSpace(newClientEmail))
                {
                    var normalizedEmail = newClientEmail.ToLower();
                    user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

                    if (user == null)
                    {
                        generatedPassword = _passwordService.Generate();
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

                var order = BuildOrder(r);
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                // Generate order number using DB id - single save after number assignment
                order.OrderNumber = $"ORD-{DateTime.Now:yyMMdd}-{order.OrdersId:D6}";

                if (r.PaymentMethodId <= 0)
                    throw new Exception("Выберите способ оплаты");

                _db.Payments.Add(new Payment
                {
                    Orders = order,
                    PaymentMethodId = r.PaymentMethodId,
                    Amount = r.Amount,
                    PaymentDate = DateTime.Now
                });

                _db.Trackings.Add(new Tracking
                {
                    OrdersId = order.OrdersId,
                    LocationTrackings = await GetTownNameAsync(order.OriginTownId),
                    UpdateDate = DateTime.Now,
                    StatusId = 2,
                    BranchesId = order.PickupBranchesId
                });

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


        public async Task<int> CreateOrderByUserAsync(CreateOrderRequest r)
        {
            ValidateOrderRequest(r);

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientsId == r.ClientId)
                    ?? throw new Exception("Клиент не найден");

                var order = BuildOrder(r);
                order.CalculatedPrice = r.CalculatedPrice;
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                order.OrderNumber = $"ORD-{DateTime.Now:yyMMdd}-{order.OrdersId:D6}";

                _db.Trackings.Add(new Tracking
                {
                    OrdersId = order.OrdersId,
                    LocationTrackings = await GetTownNameAsync(order.OriginTownId),
                    UpdateDate = DateTime.Now,
                    StatusId = 1,
                    BranchesId = order.PickupBranchesId
                });

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

        public async Task<List<Order>> GetOrdersForManagerAsync(int branchId) =>
            await OrdersWithDetails()
                .Where(o => o.PickupBranchesId == branchId || o.DestinationBranchId == branchId)
                .OrderByDescending(o => o.OrdersId)
                .ToListAsync();

        public async Task<List<Order>> GetOrdersForClientAsync(int clientId) =>
             await OrdersWithDetails()
                 .Where(o => o.ClientsId == clientId)
                 .OrderByDescending(o => o.OrdersId)
                 .ToListAsync();

         public async Task<Order?> GetOrderByIdAsync(int orderId) =>
            await _db.Orders
                .Include(o => o.Template)
                .Include(o => o.DeliveryType)
                .Include(o => o.TransportType)
                .Include(o => o.OriginTown).ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown).ThenInclude(t => t.Country)
                .Include(o => o.Clients)
                .Include(o => o.Trackings).ThenInclude(t => t.Status)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);

        public async Task<List<Order>> GetOrdersForAdminAsync(int branchId) =>
         await OrdersWithDetails()
            .Where(o => o.PickupBranchesId == branchId || o.DestinationBranchId == branchId)
            .OrderByDescending(o => o.OrdersId)
            .ToListAsync();

        public async Task<Order?> GetOrderDetailsAsync(int orderId) =>
          await _db.Orders
              .Include(o => o.DeliveryType)
              .Include(o => o.TransportType)
              .Include(o => o.OriginTown).ThenInclude(t => t.Country)
              .Include(o => o.DestinationTown).ThenInclude(t => t.Country)
              .Include(o => o.PickupBranches)
              .Include(o => o.Clients)
              .Include(o => o.Trackings).ThenInclude(t => t.Status)
              .FirstOrDefaultAsync(o => o.OrdersId == orderId);

        public async Task<List<StatusDelivery>> GetStatusDeliveriesAsync() =>
          await _db.StatusDeliveries.AsNoTracking().ToListAsync();

        public async Task<Order> GetOrderForPaymentAsync(int orderId) =>
            await _db.Orders
                .Include(o => o.Payments).ThenInclude(p => p.PaymentMethod)
                .Include(o => o.Trackings).ThenInclude(t => t.Status)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId)
            ?? throw new Exception("Заказ не найден");

        public async Task<List<PaymentMethod>> GetPaymentMethodsAsync() =>
                await _db.PaymentMethods.ToListAsync();

        public async Task<OrderEditModel?> GetOrderForEditAsync(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.OriginTown).ThenInclude(t => t.Country)
                .Include(o => o.DestinationTown).ThenInclude(t => t.Country)
                .Include(o => o.DeliveryType)
                .Include(o => o.Template)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrdersId == orderId);

            if (order == null) return null;

            return new OrderEditModel
            {
                OrdersId = order.OrdersId,
                OriginTownId = order.OriginTownId,
                DestinationTownId = order.DestinationTownId,
                DeliveryTypeId = order.DeliveryTypeId,
                TemplateId = order.TemplateId,
                OriginTownName = order.OriginTown?.TownName,
                OriginCountryName = order.OriginTown?.Country?.CountryName,
                DestinationTownName = order.DestinationTown?.TownName,
                DestinationCountryName = order.DestinationTown?.Country?.CountryName,
                DeliveryTypeName = order.DeliveryType?.NameDeliveryType,
                TotalAmount = order.Payments?.FirstOrDefault()?.Amount,
                LastRecepientName = order.LastRecepientName,
                FirstRecepientName = order.FirstRecepientName,
                MiddleRecepientName = order.MiddleRecepientName,
                PhoneRecepient = order.PhoneRecepient,
                DescriptionParcel = order.DescriptionParcel,
                LengthCm = order.LengthCm,
                WidthCm = order.WidthCm,
                HeightCm = order.HeightCm,
                Weight = order.Weight,
                CourierDestAddress = order.CourierDestAddress
            };
        }


        public async Task<bool> SaveOrderAsync(int orderId, OrderEditModel editModel)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrdersId == orderId);
            if (order == null) return false;

            order.LastRecepientName = editModel.LastRecepientName;
            order.FirstRecepientName = editModel.FirstRecepientName;
            order.MiddleRecepientName = editModel.MiddleRecepientName;
            order.PhoneRecepient = editModel.PhoneRecepient;
            order.DescriptionParcel = editModel.DescriptionParcel;

            if (order.TemplateId == null)
            {
                order.LengthCm = editModel.LengthCm;
                order.WidthCm = editModel.WidthCm;
                order.HeightCm = editModel.HeightCm;
                order.Weight = editModel.Weight;
            }

            if (order.DeliveryTypeId == 1)
                order.CourierDestAddress = editModel.CourierDestAddress;

            await _db.SaveChangesAsync();
            return true;
        }

        private static void ValidateOrderRequest(CreateOrderRequest r)
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
        }

        private static Order BuildOrder(CreateOrderRequest r) => new()
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

        private IQueryable<Order> OrdersWithDetails() =>
          _db.Orders
              .Include(o => o.Template)
              .Include(o => o.DeliveryType)
              .Include(o => o.TransportType)
              .Include(o => o.OriginTown).ThenInclude(t => t.Country)
              .Include(o => o.DestinationTown).ThenInclude(t => t.Country)
              .Include(o => o.Clients);

    }
}
