using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; } // Primary Key

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public string? StaffRole { get; set; } // Staff role (e.g., Maintenance, Admin, etc.
        public TimeOnly? ShiftStartTime { get; set; }
        public TimeOnly? ShiftEndTime { get; set; }
        public bool IsVacant { get; set; }
        public virtual User? User { get; set; }

        // Navigation properties can be added if there are relationships, e.g., Maintenance Requests assigned to the staff
        public virtual ICollection<MaintenanceRequest>? MaintenanceRequests { get; set; }

        //First shift: 6AM - 2PM
        //Second shift: 2PM - 10PM
        //Third: 10PM - 6AM  

    }
}
