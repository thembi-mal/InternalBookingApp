using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternalBookingApp.Models
{
    public class BookingModel
    {
        [Key]
        public int Id { get; set; }

        public int ResourceId { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Booked By")]
        [StringLength(150)]
        public string BookedBy { get; set; }

        [Required]
        [StringLength(150)]
        public string Purpose { get; set; }

        [ForeignKey("ResourceId")]
        public virtual ResourceManagement ResourceManagement { get; set; }

    }
}
