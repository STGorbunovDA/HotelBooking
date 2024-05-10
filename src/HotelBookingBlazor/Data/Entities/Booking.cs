using HotelBookingBlazor.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Data.Entities
{
    public class Booking
    {
        [Key]
        public long Id { get; set; }

        public short RoomTypeId { get; set; }

        public int? RoomId { get; set; }

        [Required]
        public string GuestId { get; set; }
        public int Adults { get; set; }
        public int Children {  get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }

        [Range(1, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        public DateTime BookedOn { get; set; }

        [MaxLength(150), Unicode(false)]
        public string? SpecialRequest { get; set; }

        [MaxLength(150), Unicode(false)]
        public string? Remarks { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
       
        public virtual Room Room {  get; set; }
        public virtual ApplicationUser Guest { get; set; }

        public virtual RoomType RoomType { get; set; }
    }
}
