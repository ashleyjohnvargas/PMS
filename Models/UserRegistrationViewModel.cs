
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class UserRegistrationViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; } // Tenant-specific data
        public int UnitID { get; set; } // Tenant-specific data
    }
}