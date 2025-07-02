using Microsoft.AspNetCore.Mvc;
using CustomerService.Application.DTOs;
using CustomerServicectService.Application.Services;
using CustomerService.Domain.Entities;

namespace CustomerService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerServiceImpl _customerService;

        public CustomerController(CustomerServiceImpl customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> Get(int id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                return Ok(customer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpPost("get-list")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomerByIds(List<int> ids)
        {
            var customers = await _customerService.GetCustomerByIdsAsync(ids);
            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CustomerDto customerDto)
        {
            try
            {
                await _customerService.AddAsync(customerDto);
                return CreatedAtAction(nameof(Get), new { id = customerDto.Id }, customerDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, CustomerDto customerDto)
        {
            if (id != customerDto.Id)
                return BadRequest("ID mismatch.");

            try
            {
                await _customerService.UpdateAsync(customerDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
