using ClientManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models.DTO
{
    public class CustomerCreateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public CustomerStatus Status { get; set; }

        [Required]
        public ContactDTO Contact { get; set; }
    }
}
