using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; } // Primary Key

        [ForeignKey(nameof(Lease))]
        public int? LeaseID { get; set; } // Foreign Key referencing Lease
        public DateTime? PaymentDate { get; set; } // Date of payment
        public decimal? Amount { get; set; } // Amount paid
        public string? PaymentMethod { get; set; } // E.g., Cash, Credit Card, Bank Transfer
        public string PaymentStatus { get; set; } = "Pending"; // E.g., Paid, Pending, Failed
        //public string? TransactionReference { get; set; } // Unique transaction identifier for tracking
        public virtual Lease? Lease { get; set; } // Navigation property
    }
}
