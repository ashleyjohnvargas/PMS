using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class LeaseApplication
    {
        public int? TenantID { get; set; } // Foreign Key referencing Tenant
        public int? UnitId { get; set; }
        public DateTime? LeaseStartDate { get; set; } // Or RentStartDate
        public int? LeaseDuration { get; set; } // Duration of the lease in months
        public bool? TermsAndConditions { get; set; } // This will capture the checkbox value
        public string? FullName { get; set; } // Full name of the tenant
        public DateTime? BirthDate { get; set; } // Date of birth of the tenant
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? CurrentAddress { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? EmployerName { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }
}
