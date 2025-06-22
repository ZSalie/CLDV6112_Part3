using System;
using System.ComponentModel.DataAnnotations;

namespace CloudDevPOE.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Customer contact is required.")]
        [Phone(ErrorMessage = "Please enter a valid contact number.")]
        public string? CustomerContact { get; set; }

        [Required(ErrorMessage = "An event must be selected for this booking.")]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date and time.")]
        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}
