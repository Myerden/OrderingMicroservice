using AutoMapper;
using OrderService.Api.Dto;
using OrderService.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Api.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Address, AddressDTO>().ReverseMap(); ;
            CreateMap<Product, ProductDTO>().ReverseMap(); ;
            CreateMap<Order, OrderDTO>().ReverseMap(); ;
        }
    }
}
