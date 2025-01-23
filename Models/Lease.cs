using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class Lease
    {
        [Key]
        public int LeaseID { get; set; } // Primary Key

        [ForeignKey(nameof(Tenant))] // Establish the foreign key
        public int? TenantID { get; set; } // Foreign Key referencing Tenant

        [ForeignKey(nameof(Unit))]
        public int? UnitId { get; set; }
        public DateTime? LeaseStartDate { get; set; } // Or RentStartDate
        public int? LeaseDuration { get; set; } // Duration of the lease in months
        public DateTime? LeaseEndDate { get; set; } // Or RentEndDate
        // Add this property to store the file path of the lease agreement
        public string? LeaseAgreementFilePath { get; set; } // File path of the lease agreement PDF
        public string? LeaseStatus { get; set; } = "Pending"; // Default value is Pending
        public bool? TermsAndConditions { get; set; } // This will capture the checkbox value
        public virtual Tenant? Tenant { get; set; } // Navigation property
        public virtual Unit? Unit { get; set; }
        public virtual LeaseDetails? LeaseDetails { get; set; }

    }
}
