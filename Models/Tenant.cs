using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class Tenant
    {
        [Key]
        public int TenantID { get; set; } // Primary Key

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public string? ProfilePicturePath { get; set; } // File path for the profile picture
        public bool IsActualTenant { get; set; } = false; // Default value is false
        public virtual User? User { get; set; }
        public string? PhoneNumber { get; set; }

    }
}