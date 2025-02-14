using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Domain.Entities;

namespace OrderManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var orders = await _orderService.GetOrdersByIdAsync(id);
            if (orders == null) return NotFound();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order) 
        {
            order.OrderId = Guid.NewGuid();
            order.Status = "Pendente";
            await _orderService.CreateOrderAsync(order);
            await _orderService.SendMessageBusSenderAsync(order);
            return CreatedAtAction(nameof(Get), new { id = order.OrderId }, order);
        }

    }
}
