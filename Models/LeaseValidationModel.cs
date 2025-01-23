namespace PMS.Models
{
    public class LeaseValidationModel
    {
        public int LeaseID { get; set; }
        public List<string> ValidIDs { get; set; } = new List<string>();
        public Dictionary<string, bool> IDValidationResults { get; set; } = new Dictionary<string, bool>();
        public bool LeaseAgreement { get; set; }
        public bool SecurityDeposit { get; set; }
        public string PaymentMethod { get; set; }

        // Add this property to handle the uploaded file
        public IFormFile LeaseAgreementFile { get; set; }
    }
}
