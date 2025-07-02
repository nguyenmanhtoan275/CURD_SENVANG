using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<IEnumerable<Customer>> GetCustomerByIdsAsync(List<int> ids);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}