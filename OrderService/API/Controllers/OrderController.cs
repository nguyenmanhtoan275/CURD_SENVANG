using OrderService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderServiceImpl _orderService;

        public OrdersController(OrderServiceImpl orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> Get(int id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetFiltered([FromQuery] int? customerId, [FromQuery] DateTime? date)
        {
            var orders = await _orderService.GetFilteredAsync(customerId, date);
            return Ok(orders);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult> Post(OrderDto orderDto)
        {
            try
            {
                await _orderService.AddAsync(orderDto);
                return CreatedAtAction(nameof(Get), new { id = orderDto.Id }, orderDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
                return BadRequest("ID mismatch.");

            try
            {
                await _orderService.UpdateAsync(orderDto);
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
                await _orderService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
