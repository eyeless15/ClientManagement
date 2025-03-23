using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models.DTO
{
    public class ContactDTO
    {
  
        [Phone]
        public string Phone { get; set; }
    }
}
