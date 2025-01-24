using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class Unit
    {
        [Key]
        public int UnitID { get; set; } // Primary Key
        public string? UnitName { get; set; }
        public string? UnitType { get; set; }
        public string? UnitOwner { get; set; }
        public string? Description { get; set; }
        public decimal? PricePerMonth { get; set; } // Rent or price per month
        public decimal? SecurityDeposit { get; set; }
        public string? Town { get; set; }
        public string? Location { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public int? NumberOfUnits { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public int? NumberOfGarages { get; set; }
        public int? NumberOfFloors { get; set; }
        public string UnitStatus { get; set; } = "Active";
        public string AvailabilityStatus { get; set; }
        public virtual ICollection<UnitImage>? Images { get; set; }

        public ICollection<Tenant> Tenants { get; set; }

    }
}
