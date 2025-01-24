using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class RequestFormModel
    {
        [Required]
        public string RequestType { get; set; } // Selected by the user
        public string RequestDescription { get; set; } // Inputted by the user
    }
}
