using ClientManagement.Data;
using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;
using ClientManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;

        public CustomerController(ICustomerService customerService, ApplicationDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(string name = null, string phone = null, string sortBy = "id", 
                                                      bool ascending = true, int pageNumber = 1, int pageSize = 10)
        {
            var customers = await _customerService.GetCustomersAsync(name, phone, sortBy, ascending, pageNumber, pageSize);
            return Ok(customers);
        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersAsync(string name = null, string phone = null, int pageNumber = 1, int pageSize = 10)
        //{
        //    var customers = await _customerService.GetCustomersAsync(name, phone, pageNumber, pageSize);

        //    return Ok(customers);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomerAsnyc(CustomerCreateDTO customer)
        {
            var newCustomer = await _customerService.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(CreateCustomerAsnyc), new { id = newCustomer.Id }, newCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomerAsync(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomerAsync(int id, CustomerUpdateDTO customer)
        {
            var result = await _customerService.UpdateCustomerAsync(id, customer);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
