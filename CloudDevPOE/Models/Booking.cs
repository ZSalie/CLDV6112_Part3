namespace CloudDevPOE.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public string? CustomerName { get; set; }  // Nullable

        public string? CustomerContact { get; set; }  // Nullable

        public int EventId { get; set; }

        public Event? Event { get; set; }  // Nullable

        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}