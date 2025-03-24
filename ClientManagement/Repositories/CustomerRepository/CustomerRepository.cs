using ClientManagement.Data;
using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Repositories.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync(string name, string phone, string sortBy, bool ascending, int pageNumber, int pageSize)
        {
            IQueryable<Customer> query = _context.Customers
                .Include(c => c.Contact) // Include the Contact information for phone filtering
                .AsQueryable();

            // Apply filtering by name if provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            // Apply filtering by phone if provided
            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(c => c.Contact.Phone.Contains(phone));
            }

            // Apply sorting based on `sortBy` and `ascending`
            switch (sortBy?.ToLower())
            {
                case "name":
                    query = ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
                    break;
                case "phone":
                    query = ascending ? query.OrderBy(c => c.Contact.Phone) : query.OrderByDescending(c => c.Contact.Phone);
                    break;
                default:
                    query = ascending ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id); // Default sort by Id
                    break;
            }

            // Apply pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<Customer> CreateCustomerAsnyc(CustomerCreateDTO customerDTO)
        {
            var contact = new Contact
            {
                Phone = customerDTO.Contact.Phone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };


            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();


            var customer = new Customer
            {
                Name = customerDTO.Name,
                Status = customerDTO.Status,
                ContactId = contact.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Contact = contact
            };


            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.Include(c => c.Contact).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDTO customerDTO)
        {
            var existingCustomer = await _context.Customers.Include(c => c.Contact).FirstOrDefaultAsync(c => c.Id == id);

            if (existingCustomer == null)
            {
                return false;
            }
            if (customerDTO.Name != null)
            {
                existingCustomer.Name = customerDTO.Name;
            }

            if (customerDTO.Status != null)
            {
                existingCustomer.Status = customerDTO.Status.Value;
            }

            if (customerDTO.Contact != null && customerDTO.Contact.Phone != null)
            {
                existingCustomer.Contact.Phone = customerDTO.Contact.Phone;
                existingCustomer.Contact.UpdatedAt = DateTime.UtcNow;
            }


            existingCustomer.UpdatedAt = DateTime.UtcNow;


            await _context.SaveChangesAsync();

            return true;
        }
    }
}
