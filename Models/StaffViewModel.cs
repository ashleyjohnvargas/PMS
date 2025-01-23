namespace PMS.Models
{
    public class StaffViewModel
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; } // Full name of the staff
        public string StaffRole { get; set; }
        public string Shift { get; set; } // First, Second, or Third
        public string Availability { get; set; } // Vacant or Occupied
    }
}
