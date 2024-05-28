using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Data.Entities
{
    public class Subscriber
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(50), Unicode(false)]
        public string Email { get; set;}

        public DateTime SubscriberOn { get; set; }
    }
}
