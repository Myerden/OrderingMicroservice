using OrderService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Api.Services
{
    public interface IOrderMailService
    {
        Task SendMail(OrderDTO orderDto);
    }
}
