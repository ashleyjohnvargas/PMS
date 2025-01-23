using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class UnitImage
    {
        [Key]
        public int ImageId { get; set; } // Primary key

        [ForeignKey(nameof(Unit))]
        public int? UnitId { get; set; }

        public string? FilePath { get; set; } // Path of the uploaded image

        // Navigation property for the related Product
        public virtual Unit? Unit { get; set; }
    }
}
