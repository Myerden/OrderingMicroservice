using AutoMapper;
using OrderService.Api.Configuration;
using OrderService.Api.Controllers;
using OrderService.Api.Data;
using OrderService.Api.Model;
using OrderService.Api.Repository;
using OrderService.Application.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace OrderService.Test
{
    public class OrderControllerUnitTest
    {
        private IOrderRepository repository;
        IMapper mapper;
        private DbContextOptions<OrderContext> dbContextOptions { get; set; }
        private OrderData orderData = new OrderData();
        public static string connectionString = "Server=localhost;Port=5433;User Id=admin;Password=admin;Database=OrderDB_Test;SSL Mode=Disable;";

        public OrderControllerUnitTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<OrderContext>()
                .UseNpgsql(connectionString)
                .Options;

            var context = new OrderContext(dbContextOptions);

            context.Database.Migrate();

            repository = new OrderRepository(context);

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            mapper = mapperConfig.CreateMapper();

            //insert demo data
            for (int i = 0; i < orderData.DEMO.Count; i++)
            {
                var order = mapper.Map<Order>(orderData.DEMO[i]);
                var insertedId = repository.Create(order).ConfigureAwait(false).GetAwaiter().GetResult();
                orderData.DEMO[i] = mapper.Map<OrderDTO>(order);
            }

            context.ChangeTracker.Clear();
        }

        #region Insert 

        [Fact]
        public async void InsertOrder_ShouldReturnOkResult()
        {
            var controller = new OrderController(repository, mapper);

            controller.ValidateModel(orderData.VALID);

            var data = await controller.Post(orderData.VALID);

            Assert.IsType<CreatedAtActionResult>(data);
        }

        [Fact]
        public async void InsertOrder_ShouldReturnBadRequest()
        {
            var controller = new OrderController(repository, mapper);

            controller.ValidateModel(orderData.UNVALID);

            var data = await controller.Post(orderData.UNVALID);

            Assert.IsType<BadRequestObjectResult>(data);
        }

        #endregion

        #region Get

        [Fact]
        public async void GetOrder_ShouldReturnOkResult()
        {
            var controller = new OrderController(repository, mapper);

            var id = orderData.DEMO[0].Id;

            var data = await controller.Get(id);

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void GetOrder_ShouldReturnNotFoundResult()
        {
            var controller = new OrderController(repository, mapper);

            var id = Guid.NewGuid();

            var data = await controller.Get(id);

            Assert.IsType<NotFoundObjectResult>(data);
        }

        [Fact]
        public async void GetOrder_ShouldMatchResult()
        {
            var controller = new OrderController(repository, mapper);

            var demo = orderData.DEMO[0];

            var data = await controller.Get(demo.Id) as OkObjectResult;

            Assert.IsType<OkObjectResult>(data);

            var obj = data.Value as OrderDTO;

            Assert.NotNull(obj);

            Assert.Equal(demo.CustomerId, obj.CustomerId);
            Assert.Equal(demo.Price, obj.Price);
            Assert.Equal(demo.Quantity, obj.Quantity);
            Assert.Equal(demo.Status, obj.Status);
            Assert.Equal(demo.Address.Country, obj.Address.Country);
            Assert.Equal(demo.Address.AddressLine, obj.Address.AddressLine);
            Assert.Equal(demo.Address.City, obj.Address.City);
            Assert.Equal(demo.Address.CityCode, obj.Address.CityCode);
            Assert.Equal(demo.Product.ImageUrl, obj.Product.ImageUrl);
            Assert.Equal(demo.Product.Name, obj.Product.Name);
        }

        #endregion

        #region Get All  

        [Fact]
        public async void GetAllOrder_ShouldReturnOkResult()
        {
            var controller = new OrderController(repository, mapper);

            var data = await controller.Get();

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void GetAllOrder_ShouldMatchResult()
        {
            var controller = new OrderController(repository, mapper);

            var data = await controller.Get() as OkObjectResult;

            Assert.IsType<OkObjectResult>(data);

            var obj = data.Value as List<OrderDTO>;

            Assert.NotNull(obj);

            orderData.DEMO.ForEach(demoOrder =>
            {
                var order = obj.Find(c => c.Id == demoOrder.Id);

                Assert.NotNull(order);

                Assert.Equal(demoOrder.CustomerId, order.CustomerId);
                Assert.Equal(demoOrder.Price, order.Price);
                Assert.Equal(demoOrder.Quantity, order.Quantity);
                Assert.Equal(demoOrder.Status, order.Status);
                Assert.Equal(demoOrder.Address.Country, order.Address.Country);
                Assert.Equal(demoOrder.Address.AddressLine, order.Address.AddressLine);
                Assert.Equal(demoOrder.Address.City, order.Address.City);
                Assert.Equal(demoOrder.Address.CityCode, order.Address.CityCode);
                Assert.Equal(demoOrder.Product.ImageUrl, order.Product.ImageUrl);
                Assert.Equal(demoOrder.Product.Name, order.Product.Name);
            });

        }

        #endregion

        #region Update

        [Fact]
        public async void UpdateOrder_ShouldReturnOkResult()
        {
            var controller = new OrderController(repository, mapper);

            var id = orderData.DEMO[0].Id;

            orderData.DEMO[0].Price = 46.9;
            orderData.DEMO[0].Quantity = 3;
            orderData.DEMO[0].Status = "Shipping";
            orderData.DEMO[0].Address.AddressLine = "Updated address line";
            orderData.DEMO[0].Address.City = "Kocaeli";
            orderData.DEMO[0].Address.CityCode = 41;
            orderData.DEMO[0].Address.Country = "Turkey";
            orderData.DEMO[0].Product.ImageUrl = "https://m.media-amazon.com/images/I/81nG9jxlxlS._AC_SX522_.jpg";
            orderData.DEMO[0].Product.Name = "iPad Pro";

            controller.ValidateModel(orderData.DEMO[0]);

            var updatedResult = await controller.Put(id, orderData.DEMO[0]);

            Assert.IsType<NoContentResult>(updatedResult);

            var updatedData = await controller.Get(id) as OkObjectResult;

            Assert.IsType<OkObjectResult>(updatedData);

            var updatedObj = updatedData.Value as OrderDTO;

            Assert.NotNull(updatedObj);

            Assert.Equal(updatedObj.CustomerId, orderData.DEMO[0].CustomerId);
            Assert.Equal(updatedObj.Price, orderData.DEMO[0].Price);
            Assert.Equal(updatedObj.Quantity, orderData.DEMO[0].Quantity);
            Assert.Equal(updatedObj.Status, orderData.DEMO[0].Status);
            Assert.Equal(updatedObj.Address.Country, orderData.DEMO[0].Address.Country);
            Assert.Equal(updatedObj.Address.AddressLine, orderData.DEMO[0].Address.AddressLine);
            Assert.Equal(updatedObj.Address.City, orderData.DEMO[0].Address.City);
            Assert.Equal(updatedObj.Address.CityCode, orderData.DEMO[0].Address.CityCode);
            Assert.Equal(updatedObj.Product.ImageUrl, orderData.DEMO[0].Product.ImageUrl);
            Assert.Equal(updatedObj.Product.Name, orderData.DEMO[0].Product.Name);
        }

        [Fact]
        public async void UpdateOrder_ShouldReturnBadRequest()
        {
            var controller = new OrderController(repository, mapper);

            var id = orderData.DEMO[0].Id;

            var order = new OrderDTO()
            {
                Id = id,
                Price = 7,
            };

            controller.ValidateModel(order);

            var updatedResult = await controller.Put(id, order);

            Assert.IsType<BadRequestObjectResult>(updatedResult);
        }

        [Fact]
        public async void UpdateOrder_ShouldReturnNotFoundResult()
        {
            var controller = new OrderController(repository, mapper);

            var id = Guid.NewGuid();

            var order = new OrderDTO()
            {
                Id = id,
                Price = 16.5,
                Quantity = 2,
                Status = "",
                Product = new ProductDTO()
                {
                    ImageUrl = "",
                    Name = ""
                },
                Address = new AddressDTO()
                {
                    AddressLine = "",
                    City = "",
                    CityCode = 27,
                    Country = ""
                }
            };

            var updatedResult = await controller.Put(id, order);

            Assert.IsType<NotFoundObjectResult>(updatedResult);
        }

        #endregion

        #region Delete

        [Fact]
        public async void DeleteOrder_ShouldReturnOkResult()
        {
            var controller = new OrderController(repository, mapper);

            var id = orderData.DEMO[0].Id;

            var data = await controller.Delete(id);

            Assert.IsType<NoContentResult>(data);

            data = await controller.Get(id);

            Assert.IsType<NotFoundObjectResult>(data);
        }

        [Fact]
        public async void DeleteOrder_ShouldReturnNotFoundResult()
        {
            var controller = new OrderController(repository, mapper);

            var id = Guid.NewGuid();

            var data = await controller.Delete(id);

            Assert.IsType<NotFoundObjectResult>(data);
        }

        #endregion

    }

    class OrderData
    {
        public OrderDTO VALID = new OrderDTO()
        {
            CustomerId = Guid.Parse("32083685-fb3b-408f-a589-89611219e3e4"),
            Price = 19.9,
            Quantity = 2,
            Status = "Shipping",
            Product = new ProductDTO()
            {
                ImageUrl = "https://m.media-amazon.com/images/I/81Tx3MJBkSL._AC_SX679_.jpg",
                Name = "Laptop"
            },
            Address = new AddressDTO()
            {
                AddressLine = "XYZ Street",
                City = "Istanbul",
                CityCode = 34,
                Country = "Turkey"
            }
        };

        public OrderDTO UNVALID = new OrderDTO()
        {
            CustomerId = Guid.Parse("32083685-fb3b-408f-a589-89611219e3e4"),
            Price = 16.5,
            //Quantity = 2,
            Status = "Waiting",
            Product = new ProductDTO()
            {
                ImageUrl = "https://m.media-amazon.com/images/I/61w-I-vrUkL._AC_SX679_.jpg",
                Name = "Mouse"
            },
            Address = new AddressDTO()
            {
                AddressLine = "My home address line",
                City = "Gaziantep",
                CityCode = 27,
                Country = "Turkey"
            }
        };

        public List<OrderDTO> DEMO = new List<OrderDTO>() {
            new OrderDTO()
            {
                CustomerId = Guid.Parse("ce7b3390-536a-41ef-8a30-f89e273ee21c"),
                Price = 11.5,
                Quantity = 5,
                Status = "Waiting",
                Product = new ProductDTO()
                {
                    ImageUrl = "https://m.media-amazon.com/images/I/61j17FjPhtL._AC_SX466_.jpg",
                    Name = "Alarm Clock"
                },
                Address = new AddressDTO()
                {
                    AddressLine = "My office address line",
                    City = "Ankara",
                    CityCode = 6,
                    Country = "Turkey"
                }
            },
            new OrderDTO()
            {
                CustomerId = Guid.Parse("84fcfb14-d14a-46cc-a777-d817dc4431cc"),
                Price = 16.5,
                Quantity = 2,
                Status = "Preparing",
                Product = new ProductDTO()
                {
                    ImageUrl = "https://m.media-amazon.com/images/I/81OTEKiXIML._AC_UY741_.jpg",
                    Name = "Watch"
                },
                Address = new AddressDTO()
                {
                    AddressLine = "Barajyolu",
                    City = "Adana",
                    CityCode = 1,
                    Country = "Turkey"
                }
            }
        };
    }

}
