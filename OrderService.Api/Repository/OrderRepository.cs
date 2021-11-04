using Microsoft.EntityFrameworkCore;
using OrderService.Api.Data;
using OrderService.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Api.Repository
{
    public class OrderRepository: IOrderRepository
    {
        private readonly OrderContext _dbContext;

        public OrderRepository(OrderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid?> Create(Order order)
        {
            try
            {
                order.CreatedAt = DateTime.Now;
                await _dbContext.AddAsync(order);
                await _dbContext.SaveChangesAsync();
                return order.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Update(Order order)
        {
            try
            {
                order.UpdatedAt = DateTime.Now;
                _dbContext.Entry(order).State = EntityState.Modified;
                var num = await _dbContext.SaveChangesAsync();
                return num > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var product = await Get(id);
                _dbContext.Orders.Remove(product);
                var num = await _dbContext.SaveChangesAsync();
                return num > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Order>> Get()
        {
            return await _dbContext.Orders.ToListAsync();
        }

        public async Task<Order> Get(Guid id)
        {
            return await _dbContext.Orders.Where(o => o.Id == id).Include(o => o.Product).Include(o => o.Address).FirstOrDefaultAsync();
        }

        public async Task<bool> ChangeStatus(Guid id, string status)
        {
            var order = await Get(id);
            order.Status = status;
            return await Update(order);
        }

        public async Task<bool> Validate(Guid id)
        {
            return await _dbContext.Orders.Where(o => o.Id == id).CountAsync() > 0;
        }
    }
}
