
namespace PMS.Models
{
    public class UserViewModel
    {
        public int UserID { get; set; } // Primary Key
        public string? FirstName { get; set; } // User's first name
        public string? LastName { get; set; } // User's last name
        public int? UnitId { get; set; }

        public string? Email { get; set; } // User's email
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; } = true; // Default value is true
        public string Status { get; set; } // For dropdown value
    }
}
