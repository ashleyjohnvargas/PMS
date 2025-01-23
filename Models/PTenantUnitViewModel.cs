namespace PMS.Models
{
    public class PTenantUnitViewModel
    {
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public string? Description { get; set; }
        public int NumberOfUnits { get; set; }
        public int NumberOfRooms { get; set; }
        public string? FirstImagePath { get; set; } // Path of the first image
    }
}
