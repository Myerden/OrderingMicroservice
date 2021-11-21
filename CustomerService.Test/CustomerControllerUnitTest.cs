using AutoMapper;
using CustomerService.Api.Configuration;
using CustomerService.Api.Controllers;
using CustomerService.Api.Data;
using CustomerService.Api.Model;
using CustomerService.Api.Repository;
using CustomerService.Application.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace CustomerService.Test
{
    public class CustomerControllerUnitTest
    {
        private ICustomerRepository repository;
        IMapper mapper;
        private DbContextOptions<CustomerContext> dbContextOptions { get; set; }
        private CustomerData customerData = new CustomerData();
        public static string connectionString = "Server=localhost;Port=5432;User Id=admin;Password=admin;Database=CustomerDB_Test;SSL Mode=Disable;";

        public CustomerControllerUnitTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<CustomerContext>()
                .UseNpgsql(connectionString)
                .Options;

            var context = new CustomerContext(dbContextOptions);

            context.Database.Migrate();

            repository = new CustomerRepository(context);

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            mapper = mapperConfig.CreateMapper();

            //insert demo data
            for (int i = 0; i < customerData.DEMO.Count; i++)
            {
                var customer = mapper.Map<Customer>(customerData.DEMO[i]);
                var insertedId = repository.Create(customer).ConfigureAwait(false).GetAwaiter().GetResult();
                customerData.DEMO[i] = mapper.Map<CustomerDTO>(customer);
            }

            context.ChangeTracker.Clear();
        }

        #region Insert 

        [Fact]
        public async void InsertCustomer_ShouldReturnOkResult()
        {
            var controller = new CustomerController(repository, mapper);

            controller.ValidateModel(customerData.VALID);

            var data = await controller.Post(customerData.VALID);

            Assert.IsType<CreatedAtActionResult>(data);
        }

        [Fact]
        public async void InsertCustomer_ShouldReturnBadRequest()
        {
            var controller = new CustomerController(repository, mapper);

            controller.ValidateModel(customerData.UNVALID);

            var data = await controller.Post(customerData.UNVALID);

            Assert.IsType<BadRequestObjectResult>(data);
        }

        #endregion

        #region Get

        [Fact]
        public async void GetCustomer_ShouldReturnOkResult()
        {
            var controller = new CustomerController(repository, mapper);

            var id = customerData.DEMO[0].Id;

            var data = await controller.Get(id);

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void GetCustomer_ShouldReturnNotFoundResult()
        {
            var controller = new CustomerController(repository, mapper);

            var id = Guid.NewGuid();

            var data = await controller.Get(id);

            Assert.IsType<NotFoundObjectResult>(data);
        }

        [Fact]
        public async void GetCustomer_ShouldMatchResult()
        {
            var controller = new CustomerController(repository, mapper);

            var demo = customerData.DEMO[0];

            var data = await controller.Get(demo.Id) as OkObjectResult;

            Assert.IsType<OkObjectResult>(data);

            var obj = data.Value as CustomerDTO;

            Assert.NotNull(obj);

            Assert.Equal(demo.Name, obj.Name);
            Assert.Equal(demo.Email, obj.Email);
            Assert.Equal(demo.Address.Country, obj.Address.Country);
            Assert.Equal(demo.Address.AddressLine, obj.Address.AddressLine);
            Assert.Equal(demo.Address.City, obj.Address.City);
            Assert.Equal(demo.Address.CityCode, obj.Address.CityCode);
        }

        #endregion

        #region Get All  

        [Fact]
        public async void GetAllCustomer_ShouldReturnOkResult()
        {
            var controller = new CustomerController(repository, mapper);

            var data = await controller.Get();

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void GetAllCustomer_ShouldMatchResult()
        {
            var controller = new CustomerController(repository, mapper);

            var data = await controller.Get() as OkObjectResult;

            Assert.IsType<OkObjectResult>(data);

            var obj = data.Value as List<CustomerDTO>;

            Assert.NotNull(obj);

            customerData.DEMO.ForEach(demoCustomer =>
            {
                var customer = obj.Find(c => c.Id == demoCustomer.Id);

                Assert.NotNull(customer);

                Assert.Equal(demoCustomer.Name, customer.Name);
                Assert.Equal(demoCustomer.Email, customer.Email);
                Assert.Equal(demoCustomer.Address.Country, customer.Address.Country);
                Assert.Equal(demoCustomer.Address.AddressLine, customer.Address.AddressLine);
                Assert.Equal(demoCustomer.Address.City, customer.Address.City);
                Assert.Equal(demoCustomer.Address.CityCode, customer.Address.CityCode);
            });

        }

        #endregion

        #region Update

        [Fact]
        public async void UpdateCustomer_ShouldReturnOkResult()
        {
            var controller = new CustomerController(repository, mapper);

            var id = customerData.DEMO[0].Id;

            customerData.DEMO[0].Name = "Updated customer name";
            customerData.DEMO[0].Email = "example4@gmail.com";
            customerData.DEMO[0].Address.AddressLine = "Updated address line";
            customerData.DEMO[0].Address.City = "Kýrþehir";
            customerData.DEMO[0].Address.CityCode = 40;
            customerData.DEMO[0].Address.Country = "Turkey";

            controller.ValidateModel(customerData.DEMO[0]);

            var updatedResult = await controller.Put(id, customerData.DEMO[0]);

            Assert.IsType<NoContentResult>(updatedResult);

            var updatedData = await controller.Get(id) as OkObjectResult;

            Assert.IsType<OkObjectResult>(updatedData);

            var updatedObj = updatedData.Value as CustomerDTO;

            Assert.NotNull(updatedObj);

            Assert.Equal(updatedObj.Name, customerData.DEMO[0].Name);
            Assert.Equal(updatedObj.Email, customerData.DEMO[0].Email);
            Assert.Equal(updatedObj.Address.Country, customerData.DEMO[0].Address.Country);
            Assert.Equal(updatedObj.Address.AddressLine, customerData.DEMO[0].Address.AddressLine);
            Assert.Equal(updatedObj.Address.City, customerData.DEMO[0].Address.City);
            Assert.Equal(updatedObj.Address.CityCode, customerData.DEMO[0].Address.CityCode);
        }

        [Fact]
        public async void UpdateCustomer_ShouldReturnBadRequest()
        {
            var controller = new CustomerController(repository, mapper);

            var id = customerData.DEMO[0].Id;

            var customer = new CustomerDTO()
            {
                Id = id,
                Name = "Updated name",
            };

            controller.ValidateModel(customer);

            // Email field is required
            var updatedResult = await controller.Put(id, customer);

            Assert.IsType<BadRequestObjectResult>(updatedResult);
        }

        [Fact]
        public async void UpdateCustomer_ShouldReturnNotFoundResult()
        {
            var controller = new CustomerController(repository, mapper);

            var id = Guid.NewGuid();

            var customer = new CustomerDTO()
            {
                Id = id,
                Name = "Updated name",
                Email = "example222@example.com",
                Address = new AddressDTO()
                {

                }
            };

            var updatedResult = await controller.Put(id, customer);

            Assert.IsType<NotFoundObjectResult>(updatedResult);
        }

        #endregion

        #region Delete

        [Fact]
        public async void DeleteCustomer_ShouldReturnOkResult()
        {
            var controller = new CustomerController(repository, mapper);

            var id = customerData.DEMO[0].Id;

            var data = await controller.Delete(id);

            Assert.IsType<NoContentResult>(data);

            data = await controller.Get(id);

            Assert.IsType<NotFoundObjectResult>(data);
        }

        [Fact]
        public async void DeleteCustomer_ShouldReturnNotFoundResult()
        {
            var controller = new CustomerController(repository, mapper);

            var id = Guid.NewGuid();

            var data = await controller.Delete(id);

            Assert.IsType<NotFoundObjectResult>(data);
        }

        #endregion

    }

    class CustomerData
    {
        public CustomerDTO VALID = new CustomerDTO()
        {
            Name = "Yusuf Erden",
            Email = "example@example.com",
            Address = new AddressDTO()
            {
                AddressLine = "Address line 1",
                City = "New York",
                CityCode = 25,
                Country = "United States"
            }
        };

        public CustomerDTO UNVALID = new CustomerDTO()
        {
            //Name = "Yusuf Erden",
            Email = "example1@example.com",
            Address = new AddressDTO()
            {
                AddressLine = "Address line 2",
                City = "Yozgat",
                CityCode = 66,
                Country = "Turkey"
            }
        };

        public List<CustomerDTO> DEMO = new List<CustomerDTO>() {
            new CustomerDTO()
            {
                Name = "Yusuf Erden",
                Email = "example2@example.com",
                Address = new AddressDTO()
                {
                    AddressLine = "Address line 3",
                    City = "Konya",
                    CityCode = 42,
                    Country = "Turkey"
                }
            },
            new CustomerDTO()
            {
                Name = "Yusuf Erden",
                Email = "example3@example.com",
                Address = new AddressDTO()
                {
                    AddressLine = "Address line 4",
                    City = "Zonguldak",
                    CityCode = 67,
                    Country = "Turkey"
                }
            }
        };
    }

}
