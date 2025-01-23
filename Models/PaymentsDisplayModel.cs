namespace PMS.Models
{
    public class PaymentsDisplayModel
    {
        public int? PaymentID { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
