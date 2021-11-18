using OrderService.Application.Dto;
using OrderService.Api.Repository;
using System;
using System.Threading.Tasks;

namespace OrderService.Api.Services
{
    public class OrderMailService : IOrderMailService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderMailService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task SendMail(OrderDTO orderDto)
        {
            try
            {
                
                 

            }
            catch (Exception e)
            {
                
                return;
            }

        }
    }

}
