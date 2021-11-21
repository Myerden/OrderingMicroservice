using MassTransit;
using OrderService.Application.Dto;
using OrderService.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Api.Consumer
{
    public class OrderConsumer : IConsumer<OrderDTO>
    {
        private readonly IOrderMailService _orderMailService;

        public OrderConsumer(IOrderMailService orderMailService)
        {
            _orderMailService = orderMailService;
        }

        public async Task Consume(ConsumeContext<OrderDTO> context)
        {
            await _orderMailService.SendMail(context.Message);
        }
    }
}
