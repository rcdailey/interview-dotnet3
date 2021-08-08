using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Data
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer?> UpdateCustomerAsync(Customer updatedCustomer);
    }
}
