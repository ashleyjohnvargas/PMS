using Microsoft.AspNetCore.Mvc;

namespace PMS.Models
{
    public class RegisterViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool? TermsAndConditions { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; } // DateTime Created

        public User User { get; set; }        // Represents the User object
        public Profile Profile { get; set; }  // Represents the Profile object (if needed)
    }


}
