
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class TenantViewModel
    {
        public int TenantID { get; set; } // Primary Key
        
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public string? FirstName { get; set; } // User's first name
        public string? LastName { get; set; } // User's last name

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        //public string? ProfilePicturePath { get; set; } // File path for the profile picture
        public virtual User? User { get; set; }
        //public string? FirstName { get; set; } // User's first name
        //public string? LastName { get; set; } // User's last name
        //[ForeignKey(nameof(Unit))] // Foreign key for Unit
        public int? UnitID { get; set; }
        public virtual Unit? Unit { get; set; } // Navigation property for Unit
        //public string? Email { get; set; } // User's email
        //public string? PhoneNumber { get; set; }
        //public string? Role { get; set; }
        public bool IsActive { get; set; } = true; // Default value is true
    }
}
