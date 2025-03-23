using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;

namespace ClientManagement.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomersAsync(string name, string phone, string sortBy, bool ascending, int pageNumber, int pageSize);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> CreateCustomerAsync(CustomerCreateDTO customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDTO customer);
    }
}
