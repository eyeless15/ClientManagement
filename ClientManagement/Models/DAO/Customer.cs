using ClientManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models.DAO
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerStatus Status { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
