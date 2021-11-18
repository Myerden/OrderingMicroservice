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
        private readonly IReportService _reportService;

        public OrderConsumer(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task Consume(ConsumeContext<ReportDto> context)
        {
            await _reportService.GenerateReport(context.Message);
        }
    }
}
