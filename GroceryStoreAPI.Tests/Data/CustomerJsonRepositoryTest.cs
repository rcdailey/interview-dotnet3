using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using GroceryStoreAPI.Data;
using GroceryStoreAPI.Tests.Utilities;
using NSubstitute;
using NUnit.Framework;

namespace GroceryStoreAPI.Tests.Data
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class CustomerJsonRepositoryTest
    {
        [Test, AutoMockData]
        public async Task Add_customer_increments_pk_and_adds_to_collection(
            [Frozen] IGroceryStoreJsonDataContext context,
            CustomerJsonRepository repo)
        {
            context.Customers.Returns(new List<Customer>());
            context.NextAvailablePrimaryKey = 4;

            var customer = new Customer("John");
            var result = await repo.AddCustomerAsync(customer);

            var expectedCustomer = customer with {Id = 4};
            context.Received().SaveJson();
            context.NextAvailablePrimaryKey.Should().Be(5);
            context.Customers.Should().BeEquivalentTo(expectedCustomer);
            result.Should().BeEquivalentTo(expectedCustomer);
        }

        [Test, AutoMockData]
        public async Task Customer_gets_updated_when_it_exists(
            [Frozen] IGroceryStoreJsonDataContext context,
            CustomerJsonRepository repo)
        {
            var customers = new List<Customer> {new("A", 1), new("B", 3)};
            context.Customers.Returns(customers);

            var updatedCustomer = customers[0] with {Name = "C"};
            var result = await repo.UpdateCustomerAsync(updatedCustomer);

            context.Received().SaveJson();
            result.Should().Be(updatedCustomer);
        }

        [Test, AutoMockData]
        public async Task Find_customer_with_invalid_id(
            [Frozen] IGroceryStoreJsonDataContext context,
            CustomerJsonRepository repo)
        {
            var customers = new List<Customer> {new("A", 1), new("B", 3)};
            context.Customers.Returns(customers);

            var result = await repo.GetCustomerByIdAsync(2);

            result.Should().BeNull();
        }

        [Test, AutoMockData]
        public async Task Find_customer_with_valid_id(
            [Frozen] IGroceryStoreJsonDataContext context,
            CustomerJsonRepository repo)
        {
            var customers = new List<Customer> {new("A", 1), new("B", 3)};
            context.Customers.Returns(customers);

            var result = await repo.GetCustomerByIdAsync(1);

            result.Should().Be(customers[0]);
        }

        [Test, AutoMockData]
        public async Task Get_all_customers(
            [Frozen] IGroceryStoreJsonDataContext context,
            CustomerJsonRepository repo)
        {
            var customers = new List<Customer> {new("A"), new("B")};
            context.Customers.Returns(customers);

            var result = await repo.GetAllCustomersAsync();

            result.Should().BeEquivalentTo(customers);
        }

        [Test, AutoMockData]
        public async Task No_customer_updated_when_it_does_not_exist(
            [Frozen] IGroceryStoreJsonDataContext context,
            CustomerJsonRepository repo)
        {
            var customers = new List<Customer> {new("A", 1), new("B", 3)};
            context.Customers.Returns(customers);

            var updatedCustomer = new Customer("C", 2);
            var result = await repo.UpdateCustomerAsync(updatedCustomer);

            context.DidNotReceive().SaveJson();
            result.Should().BeNull();
        }
    }
}
