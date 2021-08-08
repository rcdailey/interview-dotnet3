using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Data
{
    public class CustomerJsonRepository : ICustomerRepository
    {
        private readonly IGroceryStoreJsonDataContext _dataContext;

        public CustomerJsonRepository(IGroceryStoreJsonDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return Task.FromResult<IEnumerable<Customer>>(_dataContext.Customers);
        }

        public Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return Task.FromResult(_dataContext.Customers.FirstOrDefault(x => x.Id == id));
        }

        public Task<Customer> AddCustomerAsync(Customer customer)
        {
            var newCustomer = customer with {Id = _dataContext.NextAvailablePrimaryKey++};
            _dataContext.Customers.Add(newCustomer);
            _dataContext.SaveJson();
            return Task.FromResult(newCustomer);
        }

        public Task<Customer?> UpdateCustomerAsync(Customer updatedCustomer)
        {
            var customers = _dataContext.Customers;

            for (var i = 0; i < customers.Count; ++i)
            {
                var customer = customers[i];
                if (customer.Id == updatedCustomer.Id)
                {
                    customers[i] = updatedCustomer;
                    _dataContext.SaveJson();
                    return Task.FromResult<Customer?>(updatedCustomer);
                }
            }

            return Task.FromResult<Customer?>(null);
        }
    }
}
