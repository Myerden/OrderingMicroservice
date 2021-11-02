using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Model;
using OrderService.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;


namespace OrderService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customers = await _orderRepository.Get();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _orderRepository.Get(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                Guid orderId = await _orderRepository.Create(order);
                return CreatedAtAction(nameof(Get), new { id = orderId }, order);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Order order)
        {
            if (await _orderRepository.Validate(id) == false)
            {
                return NotFound("Order not found");
            }

            if (ModelState.IsValid)
            {
                order.Id = id;
                await _orderRepository.Update(order);
                return Ok(order);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _orderRepository.Validate(id) == false)
            {
                return NotFound("Order not found");
            }

            await _orderRepository.Delete(id);
            return Ok("Order deleted successfully");
        }

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> Put(Guid id, [FromQuery] string status)
        {
            if (await _orderRepository.Validate(id) == false)
            {
                return NotFound("Order not found");
            }
            await _orderRepository.ChangeStatus(id, status);
            return Ok("Order status changed successfully");
        }

    }
}
