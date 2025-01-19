using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; } // Primary Key
        public string? FirstName { get; set; } // User's first name
        public string? LastName { get; set; } // User's last name
        public string? Email { get; set; } // User's email

        //   [Required(ErrorMessage = "Password is required.")]
        //   [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        //   [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}",
        //ErrorMessage = "Password must contain at least one number, one uppercase letter, and one lowercase letter.")]
        public string? Password { get; set; } // User's password (hashed)
        public string? ConfirmPassword { get; set; }
        public bool? TermsAndConditions { get; set; } // This will capture the checkbox value
        public string? Role { get; set; } // Role (Property Manager, Staff, Tenant)
                                            // Set default value for IsActive in the constructor
        public bool IsActive { get; set; } = true; // Default value is true
    }
}

