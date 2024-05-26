namespace HotelBookingBlazor.Models.Public
{
    public record PaymentModel(long BookingId, string RoomTypeName, int NoOfDays, decimal TotalAmount);
}
