using ClientManagement.Models.DAO;
using ClientManagement.Repositories.CustomerRepository;
using ClientManagement.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagement.Tests.UnitTests.Repositories
{
    public class CustomerServiceTests
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<CustomerService> _customerService;

        public CustomerServiceTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _customerService = new Mock<CustomerService>(_mockCustomerRepository.Object, _mockCache.Object);
        }


        [Fact]
        public async Task GetCustomersAsync_ShouldReturnCachedCustomer_WhenCacheHit()
        {
            // Arrange
            var cacheKey = "Customers_Aleksandar_123_name_true_1_10";
            var cachedCustomers = new List<Customer> { new Customer { Id = 1, Name = "Aleksandar" } };

            _mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

            // Act
            var result = await _customerService.Object.GetCustomersAsync("Aleksandar Korisnik", "123", "name", true, 1, 10);

            // Assert
            Assert.Equal(cachedCustomers, result);
            _mockCustomerRepository.Verify(r => r.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);

            _mockCache.Verify(c => c.TryGetValue(cacheKey, out cachedCustomers), Times.Once);
        }


        [Fact]
        public async Task GetCustomersAsync_ShouldCallRepository_WhenCacheMiss()
        {
            // Arrange
            var cacheKey = "Customers_John_123_name_true_1_10";
            IEnumerable<Customer> repositoryResult = new List<Customer> { new Customer { Id = 1, Name = "John Doe", Contact = new Contact { Phone = "123435" } } };

            _mockCache.Setup(c => c.TryGetValue(cacheKey, out repositoryResult)).Returns(false);
            _mockCustomerRepository.Setup(r => r.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(repositoryResult);

            // Act
            var result = await _customerService.Object.GetCustomersAsync("John", "123", "name", true, 1, 10);

            // Assert
            Assert.Equal(repositoryResult, result);
            _mockCache.Verify(c => c.Set(cacheKey, repositoryResult, TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldApplyNameFilter()
        {
            // Arrange
            var customerList = new List<Customer> { new Customer { Id = 1, Name = "John Doe" }, new Customer { Id = 2, Name = "Jane Doe" } };
            _mockCustomerRepository.Setup(r => r.GetCustomersAsync("John", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(customerList.Where(c => c.Name.Contains("John")).ToList());

            // Act
            var result = await _customerService.Object.GetCustomersAsync("John", null, "name", true, 1, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("John Doe", result.First().Name);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldApplyPhoneFilter()
        {
            // Arrange
            var customerList = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe", Contact = new Contact { Phone = "12345" }},
            new Customer { Id = 2, Name = "Jane Doe", Contact = new Contact { Phone = "67890" }}
        };

            _mockCustomerRepository.Setup(r => r.GetCustomersAsync(It.IsAny<string>(), "12345", It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(customerList.Where(c => c.Contact.Phone.Contains("12345")).ToList());

            // Act
            var result = await _customerService.Object.GetCustomersAsync(null, "12345", "phone", true, 1, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("12345", result.First().Contact.Phone);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldSortByNameAscending()
        {
            // Arrange
            var customerList = new List<Customer>
        {
            new Customer { Id = 2, Name = "Jane Doe" },
            new Customer { Id = 1, Name = "John Doe" }
        };

            _mockCustomerRepository.Setup(r => r.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), "name", true, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(customerList.OrderBy(c => c.Name).ToList());

            // Act
            var result = await _customerService.Object.GetCustomersAsync(null, null, "name", true, 1, 10);

            // Assert
            Assert.Equal("Jane Doe", result.First().Name);
            Assert.Equal("John Doe", result.Last().Name);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldPaginateResults()
        {
            // Arrange
            var customerList = new List<Customer>
        {
            new Customer { Id = 1, Name = "John Doe" },
            new Customer { Id = 2, Name = "Jane Doe" }
        };

            _mockCustomerRepository.Setup(r => r.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), 2, 1)).ReturnsAsync(new List<Customer> { customerList[1] });

            // Act
            var result = await _customerService.Object.GetCustomersAsync(null, null, "name", true, 2, 1);

            // Assert
            Assert.Single(result);
            Assert.Equal("Jane Doe", result.First().Name);
        }
    }
}
