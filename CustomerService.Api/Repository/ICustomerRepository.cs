using CustomerService.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Api.Repository
{
    public interface ICustomerRepository
    {
        public Task<Guid?> Create(Customer customer);

        public Task<bool> Update(Customer customer);

        public Task<bool> Delete(Guid id);

        public Task<IEnumerable<Customer>> Get();

        public Task<Customer> Get(Guid id);

        public Task<bool> Validate(Guid id);

    }
}
