using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Data.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } 
        public long BookingId { get; set; }

        [MaxLength(150)]
        public string CheckoutSessionId { get; set; }

        [MaxLength(10), Unicode(false)]
        public string Status { get; set; }

        [MaxLength(150), Unicode(false)]
        public string? AdditionalInfo { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
