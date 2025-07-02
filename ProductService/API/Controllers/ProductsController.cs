using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Services;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductServiceImpl _productService;

        public ProductsController(ProductServiceImpl productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpPost("get-list")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByIds(List<int> ids)
        {
            var products = await _productService.GetProductsByIdsAsync(ids);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ProductDto productDto)
        {
            try
            {
                await _productService.AddAsync(productDto);
                return CreatedAtAction(nameof(Get), new { id = productDto.Id }, productDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest("ID mismatch.");

            try
            {
                await _productService.UpdateAsync(productDto);
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
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
