using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [ForeignKey(nameof(User))] // Establish foreign key relationship with User
        public int UserID { get; set; } // Foreign Key for User

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        // Navigation property for the User
        public virtual User? User { get; set; }
    }

}
