using System.IO.Abstractions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.ResolveAnything;
using FluentAssertions;
using GroceryStoreAPI.Controllers;
using GroceryStoreAPI.Data;
using GroceryStoreAPI.Tests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace GroceryStoreAPI.Tests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class CustomerControllerTest
    {
        private class TestContext
        {
            public TestContext(params object[] customerData)
            {
                MockFilesystem = Substitute.For<IFileSystem>();

                var builder = new ContainerBuilder();
                CompositionRoot.Setup(builder);

                // Replace the normal `IFileSystem` registration with our mock so we don't
                // have to deal with real files in the integration test. We avoid mocking
                // everything else to keep the integration tests as meaningful as possible.
                builder.RegisterInstance(MockFilesystem).As<IFileSystem>();

                // Do this now because it's required by the JSON context factory when LoadData is called.
                MockCustomerData(customerData);

                // We need to construct concrete types through Autofac to inject the
                // controller and maybe other concrete types later.
                builder.RegisterSource<AnyConcreteTypeNotAlreadyRegisteredSource>();

                Container = builder.Build();

                Controller = Container.Resolve<CustomerController>();
            }

            public IFileSystem MockFilesystem { get; }
            public IContainer Container { get; }
            public CustomerController Controller { get; }

            private void MockCustomerData(params object[] customerData)
            {
                MockFilesystem.File.OpenText(Arg.Any<string>()).Returns(_ =>
                {
                    var database = JObject.FromObject(new
                    {
                        customers = customerData
                    });

                    return StreamUtils.MakeStreamReader(database.ToString());
                });

                MockFilesystem.File.CreateText(Arg.Any<string>()).Returns(_ => StreamUtils.MakeStreamWriter());
                MockFilesystem.File.Exists(Arg.Any<string>()).Returns(true);
            }
        }

        [Test]
        public async Task Add_customer()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            var result = await ctx.Controller.NewCustomer(new Customer("New Customer"));

            result
                .Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeOfType<Customer>()
                .Which.Should().BeEquivalentTo(new Customer("New Customer", 3));
        }

        [Test]
        public async Task Get_all_customers()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            var result = await ctx.Controller.GetAllCustomers();

            result.Should().BeEquivalentTo(
                new Customer("John", 1),
                new Customer("Jane", 2));
        }

        [Test]
        public async Task Get_single_customer_http_not_found()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            var result = await ctx.Controller.GetCustomer(3);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task Get_single_customer_http_ok()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            var result = await ctx.Controller.GetCustomer(2);

            result.Result
                .Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<Customer>()
                .Which.Should().BeEquivalentTo(new Customer("Jane", 2));
        }

        [Test]
        public async Task Update_customer_http_bad_request()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            // When the ID in the URI and the request object do not match
            var result = await ctx.Controller.UpdateCustomer(3, new Customer("New Name", 2));

            result.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public async Task Update_customer_http_not_found()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            var result = await ctx.Controller.UpdateCustomer(3, new Customer("New Name", 3));

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task Update_customer_http_ok()
        {
            var ctx = new TestContext(
                new
                {
                    name = "John",
                    id = 1
                },
                new
                {
                    name = "Jane",
                    id = 2
                });

            var result = await ctx.Controller.UpdateCustomer(2, new Customer("New Name", 2));

            result
                .Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<Customer>()
                .Which.Should().BeEquivalentTo(new Customer("New Name", 2));
        }
    }
}
