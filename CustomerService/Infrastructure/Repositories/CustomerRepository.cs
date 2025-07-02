using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Data;

namespace CustomerService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id) ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task AddAsync(Customer product)
        {
            await _context.Customers.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer product)
        {
            _context.Customers.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Customers.FindAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            _context.Customers.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomerByIdsAsync(List<int> ids)
        {
            return await _context.Customers.Where(x => ids.Contains(x.Id)).ToListAsync();
        }
    }
}
