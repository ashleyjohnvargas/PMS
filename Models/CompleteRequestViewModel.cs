namespace PMS.Models
{
    public class CompleteRequestViewModel
    {
        public int RequestId { get; set; } // Matches the hidden input field in the form
        public DateTime Date { get; set; } // Matches the "Date" field in the form
        public TimeSpan Time { get; set; } // Matches the "Time" field in the form
        public string? Comment { get; set; } // Matches the "Comment" field in the form
        public decimal Cost { get; set; } // Matches the "Cost" field in the form
    }
}
