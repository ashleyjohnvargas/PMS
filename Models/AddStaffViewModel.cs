using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class AddStaffViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Electrician, Technician, etc.

        [Required]
        public string Shift { get; set; } // First, Second, or Third
    }
}
