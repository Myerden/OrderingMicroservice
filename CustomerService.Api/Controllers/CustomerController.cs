using AutoMapper;
using CustomerService.Api.Dto;
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
        private readonly IMapper _mapper;

        public CustomerController(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customers = await _customerRepository.Get();
            var customersDTO = _mapper.Map<IEnumerable<CustomerDTO>>(customers);
            return Ok(customersDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _customerRepository.Get(id);
            if(customer == null)
            {
                return NotFound("Customer not found");
            }
            var customerDTO = _mapper.Map<CustomerDTO>(customer);
            return Ok(customerDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerDTO customerDTO)
        {
            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Customer>(customerDTO);
                await _customerRepository.Create(customer);
                customerDTO = _mapper.Map<CustomerDTO>(customer);
                return CreatedAtAction(nameof(Get), new { id = customerDTO.Id }, customerDTO);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] CustomerDTO customerDTO)
        {
            if(await _customerRepository.Validate(id) == false)
            {
                return NotFound("Customer not found");
            }

            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Customer>(customerDTO);
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
