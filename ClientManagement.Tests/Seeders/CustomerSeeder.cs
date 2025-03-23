using ClientManagement.Data;
using ClientManagement.Models.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagement.Tests.Seeders
{
    public static class CustomerSeeder
    {
        public static void CreateCustomers(ApplicationDbContext context)
        {
            context.Customers.AddRange(new[]
            {
                new Customer
            {
                Id = 1,
                Name = "Test user 1",
                Contact = new Contact()
                { Id = 1,
                 Phone = "123456789",
                 CreatedAt = DateTime.UtcNow,
                 UpdatedAt = DateTime.UtcNow,
                },
                ContactId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
                }
            });

            context.SaveChangesAsync();
        }
    }
}
