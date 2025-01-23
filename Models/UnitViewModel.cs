namespace PMS.Models
{
    public class UnitViewModel
    {
        public int? UnitId { get; set; }
        public string? UnitName { get; set; }
        public decimal? PricePerMonth { get; set; }
        public string? UnitType { get; set; } // added
        public string? UnitStatus { get; set; }
    }
}
