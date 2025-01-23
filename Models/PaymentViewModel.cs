namespace PMS.Models
{
    public class PaymentViewModel
    {
        public int? LeaseId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? MonthlyRent { get; set; }
        public string? PaymentMethod { get; set; }

    }
}
