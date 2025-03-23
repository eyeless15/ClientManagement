using ClientManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models.DTO
{
    public class CustomerUpdateDTO
    {
        public string? Name { get; set; }

        public CustomerStatus? Status { get; set; }

        public ContactDTO? Contact { get; set; }
    }
}
