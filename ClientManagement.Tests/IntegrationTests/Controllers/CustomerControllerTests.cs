using System.Net;
using ClientManagement.Data;
using ClientManagement.Models.DAO;
using ClientManagement.Models.DTO;
using ClientManagement.Tests.Seeders;
using System.Net.Http.Json;
using FluentAssertions;


namespace ClientManagement.Tests.IntegrationTests.Controllers
{
    public class CustomerControllerTests : IntegrationTest
    {


        public CustomerControllerTests() : base()
        {
            
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnsFound_WhenCustomerExists()
        {

            // Act
            var response = await TestClient.GetAsync("/api/Customer/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            customer.Should().NotBeNull();
            customer.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Act
            var response = await TestClient.GetAsync("/api/Customer/999"); // Assume this ID doesn't exist

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateCustomerAsync_ReturnsCreated_WhenCustomerIsValid()
        {
            // Arrange
            var customerToCreate = new CustomerCreateDTO
            {
                Name = "New Customer",
                Contact = new ContactDTO
                {
                    Phone = "987654321"
                }
            };

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/Customer", customerToCreate);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCustomer = await response.Content.ReadFromJsonAsync<Customer>();
            createdCustomer.Should().NotBeNull();
            createdCustomer.Name.Should().Be(customerToCreate.Name);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ReturnsNoContent_WhenCustomerExists()
        {
            // Act
            var response = await TestClient.DeleteAsync("/api/Customer/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the customer is deleted by attempting to get the customer
            var getResponse = await TestClient.GetAsync("/api/Customer/1");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Act
            var response = await TestClient.DeleteAsync("/api/Customer/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ReturnsNoContent_WhenCustomerExists()
        {
            //Arange
            var customerToUpdate = new CustomerUpdateDTO
            {
                Name = "Updated Customer",
                Contact = new ContactDTO
                {
                    Phone = "1122334455",
                }
            };

            // Act
            var response = await TestClient.PutAsJsonAsync("/api/Customer/1", customerToUpdate);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the update by checking the customer data
            var getResponse = await TestClient.GetAsync("/api/Customer/1");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedCustomer = await getResponse.Content.ReadFromJsonAsync<Customer>();

            updatedCustomer.Name.Should().Be("Updated Customer");
            updatedCustomer.Contact.Phone.Should().Be("1122334455");
        }

        [Fact]
        public async Task UpdateCustomerAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerToUpdate = new CustomerUpdateDTO
            {
                Name = "Non-Existent Customer",
                Contact = new ContactDTO
                {
                    Phone = "4353459887",
                }
            };

            // Act
            var response = await TestClient.PutAsJsonAsync("/api/Customer/999", customerToUpdate); 

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
