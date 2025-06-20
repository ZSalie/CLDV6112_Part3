using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CloudDevPOE.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [MinLength(3)]
        public string UserName { get; set; }
        [EmailAddress]
        public string UserEmail { get; set; }
        [DataType(DataType.Password)]
        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; }

    }
}
