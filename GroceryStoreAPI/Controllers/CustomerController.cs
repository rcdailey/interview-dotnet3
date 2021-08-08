using System.Collections.Generic;
using System.Threading.Tasks;
using GroceryStoreAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroceryStoreAPI.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> GetAllCustomers() =>
            await _customerRepository.GetAllCustomersAsync();

        [HttpGet("{id}"),
         ProducesResponseType(StatusCodes.Status404NotFound),
         ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Customer?>> GetCustomer(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            return customer is null ? NotFound() : Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> NewCustomer(Customer customerToAdd)
        {
            var newCustomer = await _customerRepository.AddCustomerAsync(customerToAdd);
            return CreatedAtAction(nameof(GetCustomer), new {id = newCustomer.Id}, newCustomer);
        }

        [HttpPut("{id}"),
         ProducesResponseType(StatusCodes.Status404NotFound),
         ProducesResponseType(StatusCodes.Status400BadRequest),
         ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            if (id != updatedCustomer.Id)
            {
                return BadRequest();
            }

            var result = await _customerRepository.UpdateCustomerAsync(updatedCustomer);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
