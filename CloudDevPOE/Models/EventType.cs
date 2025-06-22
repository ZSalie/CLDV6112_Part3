using System.ComponentModel.DataAnnotations;

namespace CloudDevPOE.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required(ErrorMessage = "Event type description is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 100 characters.")]
        public string Description { get; set; } = null!;
    }
}
