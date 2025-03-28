using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("ordenes")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _orderService.GetOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            _orderService.CreateOrder(order);

            var response = new
            {
                id = order.Id,
                cliente = order.Cliente,
                fechaCreacion = order.FechaCreacion,
                total = order.Total,
                products = order.OrderProducts.Select(op => new
                {
                    productId = op.ProductId,
                    quantity = op.Quantity
                })
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id) return BadRequest();
            _orderService.UpdateOrder(order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            _orderService.DeleteOrder(id);
            return NoContent();
        }
    }
}
