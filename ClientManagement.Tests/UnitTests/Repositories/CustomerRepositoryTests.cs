using Azure.Core;
using ClientManagement.Data;
using ClientManagement.Enums;
using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;
using ClientManagement.Repositories.CustomerRepository;
using ClientManagement.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagement.Tests.UnitTests.Repositories
{
    public class CustomerRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerRepository _customerRepository;

        public CustomerRepositoryTests()
        {
            // Setup InMemory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Ensure unique database for each test
                .Options;

            _context = new ApplicationDbContext(options);
            _customerRepository = new CustomerRepository(_context);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldReturnFilteredByName()
        {
            // Arrange
            AddSampleCustomers();

            var nameFilter = "Aleksandar";
            var phoneFilter = "";
            var sortBy = "name";
            var ascending = true;
            var pageNumber = 1;
            var pageSize = 10;


            await _context.SaveChangesAsync();

            // Act
            var result = await _customerRepository.GetCustomersAsync(nameFilter, phoneFilter, sortBy, ascending, pageNumber, pageSize);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, c => c.Name.Contains(nameFilter));
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldReturnFilteredByPhone()
        {
            // Arrange
            AddSampleCustomers();

            var nameFilter = "";
            var phoneFilter = "12345";
            var sortBy = "phone";
            var ascending = true;
            var pageNumber = 1;
            var pageSize = 10;



            // Act
            var result = await _customerRepository.GetCustomersAsync(nameFilter, phoneFilter, sortBy, ascending, pageNumber, pageSize);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, c => c.Contact.Phone.Contains(phoneFilter));
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldSortAscendingByName()
        {
            // Arrange
            AddSampleCustomers();

            var nameFilter = "";
            var phoneFilter = "";
            var sortBy = "name";
            var ascending = true;
            var pageNumber = 1;
            var pageSize = 10;

            // Act
            var result = await _customerRepository.GetCustomersAsync(nameFilter, phoneFilter, sortBy, ascending, pageNumber, pageSize);

            // Assert
            var customerList = result.ToList();
            Assert.Equal("Aleksandar Korisnik", customerList[0].Name); 
            Assert.Equal("Fedex", customerList[1].Name); 
            Assert.Equal("Lorna Shore", customerList[2].Name);   
        }


        [Fact]
        public async Task GetCustomersAsync_ShouldSortDescendingByPhone()
        {
            // Arrange
            AddSampleCustomers();  

            var nameFilter = "";
            var phoneFilter = "";
            var sortBy = "phone";
            var ascending = false;
            var pageNumber = 1;
            var pageSize = 10;

            // Act
            var result = await _customerRepository.GetCustomersAsync(nameFilter, phoneFilter, sortBy, ascending, pageNumber, pageSize);

            // Assert
            var customerList = result.ToList();
            Assert.Equal("67890", customerList[0].Contact.Phone); 
            Assert.Equal("54321", customerList[1].Contact.Phone); 
            Assert.Equal("12345", customerList[2].Contact.Phone);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldApplyPagination()
        {
            // Arrange
            AddSampleCustomers(); 

            var nameFilter = "";
            var phoneFilter = "";
            var sortBy = "id";
            var ascending = true;
            var pageNumber = 1;
            var pageSize = 2;

            // Act
            var result = await _customerRepository.GetCustomersAsync(nameFilter, phoneFilter, sortBy, ascending, pageNumber, pageSize);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == 1);
            Assert.Contains(result, c => c.Id == 2);
        }


        [Fact]
        public async Task CreateCustomerAsync_ShouldReturnCustomer()
        {
            // Arrange
            var customerDTO = GetSampleCustomerDTO();

            // Act
            var result = await _customerRepository.CreateCustomerAsnyc(customerDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unit Test Customer", result.Name);
            Assert.Equal(CustomerStatus.Neaktivan, result.Status);
            Assert.NotNull(result.Contact);
            Assert.Equal("123456789", result.Contact.Phone);

            //Make sure that customer was saved in database
            var customerSaved = await _context.Customers.FirstOrDefaultAsync(c => c.Id == result.Id);
            Assert.NotNull(customerSaved);
        }


        [Fact]
        public async Task DeleteCustomerAsync_ShouldDeleteCustomer_WhenExists()
        {
            //Arrange

            var customer = new Customer
            {
                Name = "Customer for delete",
                Status = CustomerStatus.Aktivan,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Contact = new Contact { Phone = "123456789"}
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            //Act
            var result = await _customerRepository.DeleteCustomerAsync(customer.Id);

            //Assert
            Assert.True(result);
            var customerInDb = await _context.Customers.FindAsync(customer.Id);
            Assert.Null(customerInDb);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _customerRepository.DeleteCustomerAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Aleksandar",
                Status = CustomerStatus.ProbniPeriod,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Contact = new Contact { Phone = "123456789" }
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerRepository.GetCustomerByIdAsync(customer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Aleksandar", result.Name);
            Assert.Equal(CustomerStatus.ProbniPeriod, result.Status);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnTrue_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer
            {
                Name = "Test User",
                Status = CustomerStatus.Aktivan,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Contact = new Contact { Phone = "123456789" }
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var customerDTO = new CustomerUpdateDTO
            {
                Name = "Test User Updated",
                Status = CustomerStatus.Aktivan,
                Contact = new ContactDTO { Phone = "987654321" }
            };

            // Act
            var result = await _customerRepository.UpdateCustomerAsync(customer.Id, customerDTO);

            // Assert
            Assert.True(result);
            var updatedCustomer = await _context.Customers.FindAsync(customer.Id);
            Assert.Equal("Test User Updated", updatedCustomer.Name);
            Assert.Equal(CustomerStatus.Aktivan, updatedCustomer.Status);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerDTO = new CustomerUpdateDTO { Name = "User Updated" };

            // Act
            var result = await _customerRepository.UpdateCustomerAsync(999, customerDTO);

            // Assert
            Assert.False(result);
        }


        private CustomerCreateDTO GetSampleCustomerDTO()
        {
            return new CustomerCreateDTO
            {
                Name = "Unit Test Customer",
                Status = CustomerStatus.Neaktivan,
                Contact = new ContactDTO { Phone = "123456789" }
            };
        }


        private void AddSampleCustomers()
        {
            _context.Customers.AddRange(
               new Customer { Id = 1, Name = "Aleksandar Korisnik", Contact = new Contact { Phone = "12345" } },
               new Customer { Id = 2, Name = "Fedex", Contact = new Contact { Phone = "67890" } },
               new Customer { Id = 3, Name = "Lorna Shore", Contact = new Contact { Phone = "54321" } }
           );
            _context.SaveChanges();
        }
    }
}
