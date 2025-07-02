using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;

namespace CustomerServicectService.Application.Services
{
    public class CustomerServiceImpl
    {
        private readonly ICustomerRepository _repository;

        public CustomerServiceImpl(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email
            };
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(p => new CustomerDto
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                Email= p.Email
            });
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomerByIdsAsync(List<int> ids)
        {
            var customers = await _repository.GetCustomerByIdsAsync(ids);
            return customers.Select(p => new CustomerDto
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                Email = p.Email
            });
        }

        public async Task AddAsync(CustomerDto customerDto)
        {
            var customer = new Customer
            {
                Name = customerDto.Name,
                Phone = customerDto.Phone,
                Email = customerDto.Email
            };
            await _repository.AddAsync(customer);
        }

        public async Task UpdateAsync(CustomerDto customerDto)
        {
            var customer = new Customer
            {
                Id = customerDto.Id,
                Name = customerDto.Name,
                Phone = customerDto.Phone,   
                Email = customerDto.Email
            };
            await _repository.UpdateAsync(customer);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
