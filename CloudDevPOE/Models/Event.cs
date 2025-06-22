using System.ComponentModel.DataAnnotations;

namespace CloudDevPOE.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [MinLength(3, ErrorMessage = "Your event name cannot be less than 3 characters.")]
        [StringLength(100, ErrorMessage = "Your event name cannot be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Event description is required.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid start date.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid end date.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Venue selection is required.")]
        public int VenueId { get; set; }

        public Venue? Venue { get; set; }  // Nullable as originally written

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [Required(ErrorMessage = "Event type is required.")]
        public int EventTypeId { get; set; }

        public EventType? EventType { get; set; } = null!;
        
    }
}
