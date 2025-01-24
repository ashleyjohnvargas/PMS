namespace PMS.Models
{
    public class MaintenanceAssignmentViewModel
    {
        public int RequestID { get; set; }
        public string UnitName { get; set; }
        public string DateRequested { get; set; }
        public string RequestTask { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string TenantName { get; set; } // New property for tenant's name

    }
}
