using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudDevPOE.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Venue Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Location must be between 5 and 200 characters.")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 100,000.")]
        public int Capacity { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }  // Upload
        [NotMapped]
        public String? ImageBase64 { get; set; } // Download

        [Required(ErrorMessage = "Creation Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
