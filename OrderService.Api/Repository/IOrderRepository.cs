using OrderService.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Api.Repository
{
    public interface IOrderRepository
    {
        public Task<Guid> Create(Order order);

        public Task<bool> Update(Order order);

        public Task<bool> Delete(Guid id);

        public Task<IEnumerable<Order>> Get();

        public Task<Order> Get(Guid id);

        public Task<bool> ChangeStatus(Guid id, string status);

        public Task<bool> Validate(Guid id);

    }
}
