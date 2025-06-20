using System.ComponentModel.DataAnnotations;

namespace CloudDevPOE.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [MinLength(3,ErrorMessage ="Your event name cannot be less than 3 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Event Description is required.")]
        public string Description { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int VenueId { get; set; }

        public Venue? Venue { get; set; }  // Nullable

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}