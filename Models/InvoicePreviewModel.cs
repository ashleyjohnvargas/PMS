namespace PMS.Models
{
    public class InvoicePreviewModel
    {
        public int? PaymentId { get; set; }
        public string? LeaseNumber { get; set; }
        public string? UnitName { get; set; }
        public decimal? MonthlyRent { get; set; }
        public string? TenantName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentTime { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
