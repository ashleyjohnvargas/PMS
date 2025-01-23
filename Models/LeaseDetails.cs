using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PMS.Models
{
    public class LeaseDetails
    {
        [Key]
        public int LeaseDetailsId { get; set; } // Primary Key

        [ForeignKey(nameof(Tenant))] // Establish the foreign key
        public int? LeaseID { get; set; } // Foreign Key referencing Tenant
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