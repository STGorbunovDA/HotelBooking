using HotelBookingBlazor.Data;
using HotelBookingBlazor.Data.Entities;
using HotelBookingBlazor.Models;
using HotelBookingBlazor.Models.Public;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services
{
    public interface IBookingService
    {
        Task<MethodResult<long>> MakeBookingAsync(BookingModel bookingModel, string userId);
    }

    public class BookingService : IBookingService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BookingService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MethodResult<long>> MakeBookingAsync(BookingModel bookingModel, string userId)
        {
            try
            {
                var booking = new Booking
                {
                    Adults = bookingModel.Adults ?? 0,
                    BookedOn = DateTime.Now,
                    CheckInDate = bookingModel.CheckInDate,
                    CheckOutDate = bookingModel.CheckOutDate,
                    Children = bookingModel.Children ?? 0,
                    GuestId = userId,
                    RoomTypeId = bookingModel.RoomTypeId,
                    SpecialRequest = bookingModel.SpecialRequest,
                    Status = Constants.BookingStatus.Pending,
                    TotalAmount = bookingModel.Amount
                };

                using var context = _contextFactory.CreateDbContext();
                await context.AddAsync(booking);
                await context.SaveChangesAsync();
                return booking.Id;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
