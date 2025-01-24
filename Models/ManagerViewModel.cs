namespace PMS.Models
{
    public class ManagerViewModel
    {
        public int ManagerID { get; set; }
        public string ManagerName { get; set; } // Full name of the staff
        //public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }
    }
}
