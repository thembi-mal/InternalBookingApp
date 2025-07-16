using System.ComponentModel.DataAnnotations;

namespace InternalBookingApp.Models
{
    public class ResourceManagement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(750)]
        public string Description { get; set; }

        [StringLength(150)]
        public string Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than one")]
        public int Capacity { get; set; }

        public Boolean IsAvailable { get; set; } = true;

        public virtual ICollection<BookingModel> BookingModels { get; set; } = new List<BookingModel>();




    }
}
