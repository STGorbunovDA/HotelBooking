using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingBlazor.Data.Entities
{
    public class Enquiry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required, MaxLength(20), Unicode(false)]
        public string Name { get; set; }

        [Required, MaxLength(50), EmailAddress, Unicode(false)]
        public string Email { get; set; }

        [Required, MaxLength(50), Unicode(false)]
        public string Subject { get; set; }

        [Required, MaxLength(250), Unicode(false)]
        public string Message { get; set; }

        public DateTime EnquiredOn { get; set; }
    }
}
