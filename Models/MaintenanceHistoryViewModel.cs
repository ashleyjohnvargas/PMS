namespace PMS.Models
{
    public class MaintenanceHistoryViewModel
    {
        public int RequestID { get; set; }
        public string UnitName { get; set; } // Added UnitName property
        public string DateRequested { get; set; }
        public string RequestTask { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string DateStarted { get; set; } // New property for the start date of the request
        public string DateFinished { get; set; } // New property for the completion date of the request
    }
}
