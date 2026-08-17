using LogisticCompany.Application.Interfaces;
using LogisticCompany.Application.Services;
using LogisticCompany.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LogisticCompany.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITrackingService _trackingService;
        private readonly IClientService _clientService;

        public OrdersController(
            IOrderService orderService,
            ITrackingService trackingService,
            IClientService clientService)
        {
            _orderService = orderService;
            _trackingService = trackingService;
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email)) return Unauthorized();

                var client = await _clientService.GetClientByEmailAsync(email);
                if (client == null) return NotFound("Клиент не найден");

                var orders = await _orderService
                    .GetOrderSummariesForClientAsync(client.ClientsId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderDetailsDtoAsync(id);
                if (order == null) return NotFound("Заказ не найден");
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}/tracking")]
        public async Task<IActionResult> GetTracking(int id)
        {
            try
            {
                var trackings = await _trackingService.GetOrderTrackingsAsync(id);
                return Ok(trackings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _orderService.GetStatusDeliveriesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
