using HotelBookingBlazor.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models
{
    public class BookingDisplayModel
    {
        public long Id { get; set; }
        public short RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int? RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string GuestId { get; set; }
        public string GuestName { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookedOn { get; set; }
        public string? SpecialRequest { get; set; }
        public string? Remarks { get; set; }

        public BookingStatus Status { get; set; }

        public bool IsRoomNumberAssignable => Status == BookingStatus.PaymentSuccess
                                           || Status == BookingStatus.Booked;
        public bool CanBeApproved => Status == BookingStatus.PaymentSuccess;

        public bool CanBeCancelled => Status == BookingStatus.PaymentCancelled
                                   && Status != BookingStatus.Cancelled;
    }
}
