using CustomerService.Api.Model;
using CustomerService.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;


namespace CustomerService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customers = await _customerRepository.Get();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _customerRepository.Get(id);
            if(customer == null)
            {
                return NotFound("Customer not found");
            }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.Create(customer);
                return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Customer customer)
        {
            if(await _customerRepository.Validate(id) == false)
            {
                return NotFound("Customer not found");
            }

            if (ModelState.IsValid)
            {
                customer.Id = id;
                await _customerRepository.Update(customer);
                return Ok("Customer updated successfully");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _customerRepository.Validate(id) == false)
            {
                return NotFound("Customer not found");
            }

            await _customerRepository.Delete(id);
            return Ok("Customer deleted successfully");
        }
    }
}
