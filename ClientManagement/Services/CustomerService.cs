using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;
using ClientManagement.Repositories.CustomerRepository;
using Microsoft.Extensions.Caching.Memory;

namespace ClientManagement.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMemoryCache _cache;

        public CustomerService(ICustomerRepository customerRepository, IMemoryCache cache)
        {
            _customerRepository = customerRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync(string name, string phone, string sortBy, bool ascending, int pageNumber, int pageSize)
        {
            // Define a cache key based on the filter criteria and pagination
            var cacheKey = $"Customers_{name}_{phone}_{sortBy}_{ascending}_{pageNumber}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Customer> customers))
            {
                customers = await _customerRepository.GetCustomersAsync(name, phone, sortBy, ascending, pageNumber, pageSize);

                _cache.Set(cacheKey, customers, TimeSpan.FromMinutes(5));
            }

            return customers;
        }
        public async Task<Customer> CreateCustomerAsync(CustomerCreateDTO customer)
        {
            return await _customerRepository.CreateCustomerAsnyc(customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            return await _customerRepository.DeleteCustomerAsync(id);
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
           return await _customerRepository.GetCustomerByIdAsync(id);
        }


        public async Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDTO customer)
        {
            return await _customerRepository.UpdateCustomerAsync(id, customer);
        }
    }
}
