namespace PMS.Models
{
    public class TenantLeaseViewModel
    {
        public int? LeaseID { get; set; }
        // Property Information
        public string? PropertyName { get; set; }
        public string? Address { get; set; }

        // Lease Details
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public decimal? MonthlyRent { get; set; }
        public string? SecurityDeposit { get; set; }
        public string? Status { get; set; }
        public string? DueDate { get; set; } // New Property for Due Date

        // Tenant Information
        public string? TenantName { get; set; }
        public string? Contact { get; set; }
        public string? Phone { get; set; }
    }
}
