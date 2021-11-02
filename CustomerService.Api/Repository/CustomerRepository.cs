using CustomerService.Api.Data;
using CustomerService.Api.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Api.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _dbContext;

        public CustomerRepository(CustomerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Create(Customer customer)
        {
            customer.CreatedAt = DateTime.Now;
            await _dbContext.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
            return customer.Id;
        }

        public async Task<bool> Update(Customer customer)
        {
            customer.UpdatedAt = DateTime.Now;
            _dbContext.Entry(customer).State = EntityState.Modified;
            var num = await _dbContext.SaveChangesAsync();
            return num > 0;
        }

        public async Task<bool> Delete(Guid id)
        {
            var product = await Get(id);
            _dbContext.Customers.Remove(product);
            var num = await _dbContext.SaveChangesAsync();
            return num > 0;
        }

        public async Task<IEnumerable<Customer>> Get()
        {
            return await _dbContext.Customers.Include(c => c.Address).ToListAsync();
        }

        public async Task<Customer> Get(Guid id)
        {
            return await _dbContext.Customers.Where(c => c.Id == id).Include(c => c.Address).FirstOrDefaultAsync();
        }

        public async Task<bool> Validate(Guid id)
        {
            return await _dbContext.Customers.Where(c => c.Id == id).CountAsync() > 0;
        }

    }
}
