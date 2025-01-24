using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; } // auto generated
        [ForeignKey(nameof(Tenant))]
        public int? TenantID { get; set; } // based on the user's session id 
        [ForeignKey(nameof(Staff))]
        public int? StaffID { get; set; } // auto generated 
        public string? RequestType { get; set; } // input based
        public string? RequestDescription { get; set; } // input based
        public DateTime? RequestDateTime { get; set; } // auto generated     
        public string RequestStatus { get; set; } // Pending, Ongoing, Completed
        public DateTime? RequestStartDateTime { get; set; } // Start date and time of request is null when a request is pending
        public DateTime? CompletedDateTime { get; set; } // CompletedDateTime is null when request is pending and ongoing
        public decimal? Cost { get; set; } // Cost of the request


        public virtual Tenant? Tenant { get; set; }
        public virtual Staff? Staff { get; set; }
    }
}
