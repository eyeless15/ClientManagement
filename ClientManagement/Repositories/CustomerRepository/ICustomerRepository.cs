using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;

namespace ClientManagement.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomersAsync(string name, string phone, string sortBy, bool ascending, int pageNumber, int pageSize);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> CreateCustomerAsnyc(CustomerCreateDTO customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDTO customer);
    }
}
