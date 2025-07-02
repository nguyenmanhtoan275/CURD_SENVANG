using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services
{
    public class ProductServiceImpl
    {
        private readonly IProductRepository _repository;

        public ProductServiceImpl(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(List<int> ids)
        {
            var products = await _repository.GetProductsByIdsAsync(ids);
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
        }
        
        public async Task AddAsync(ProductDto productDto)
        {
            if (productDto.Price < 0)
                throw new ArgumentException("Price cannot be negative.");

            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price
            };
            await _repository.AddAsync(product);
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            if (productDto.Price < 0)
                throw new ArgumentException("Price cannot be negative.");

            var product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Price = productDto.Price
            };
            await _repository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
