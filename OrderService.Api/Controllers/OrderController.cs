using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Dto;
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
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderRepository.Get();
            var ordersDTO = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            return Ok(ordersDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _orderRepository.Get(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _mapper.Map<Order>(orderDTO);
            await _orderRepository.Create(order);
            orderDTO = _mapper.Map<OrderDTO>(order);
            return CreatedAtAction(nameof(Get), new { id = orderDTO.Id }, orderDTO);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] OrderDTO orderDTO)
        {
            if (await _orderRepository.Validate(id) == false)
            {
                return NotFound("Order not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderDTO.Id)
            {
                return BadRequest();
            }

            var order = _mapper.Map<Order>(orderDTO);
            await _orderRepository.Update(order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _orderRepository.Validate(id) == false)
            {
                return NotFound("Order not found");
            }

            await _orderRepository.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}/change-status")]
        public async Task<IActionResult> Put(Guid id, [FromQuery] string status)
        {
            if (await _orderRepository.Validate(id) == false)
            {
                return NotFound("Order not found");
            }
            await _orderRepository.ChangeStatus(id, status);
            return NoContent();
        }

    }
}
