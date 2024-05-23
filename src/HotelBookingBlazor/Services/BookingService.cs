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
        Task<PagedResult<BookingDisplayModel>> GetBookingAsync(int startIndex, int pageSize);
        Task<MethodResult> ApproveBookingAsync(long bookingId);
        Task<MethodResult> CancelBookingAsync(long bookingId, string cancelReason);
        Task<PagedResult<BookingDisplayModel>> GetBookingsForGuestAsync(string guestId, BookingDisplayModel type, int startIndex, int pageSize);
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
   
        public async Task<PagedResult<BookingDisplayModel>> GetBookingAsync(int startIndex, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Bookings;

            var totalCount = await query.CountAsync();

            var bookings = await query.OrderByDescending(b => b.CheckInDate)
                                      .Select(b => new BookingDisplayModel
                                      {
                                          Adults = b.Adults,
                                          BookedOn = b.BookedOn,
                                          CheckInDate = b.CheckInDate,
                                          CheckOutDate = b.CheckOutDate,
                                          Children = b.Children,
                                          GuestId = b.GuestId,
                                          GuestName = b.Guest.FullName,
                                          RoomTypeId = b.RoomTypeId,
                                          RoomTypeName = b.RoomType.Name,
                                          Id = b.Id,
                                          RoomId = b.RoomId,
                                          RoomNumber = b.RoomId == null ? "" : b.Room.RoomNumber,
                                          SpecialRequest = b.SpecialRequest,
                                          Status = b.Status,
                                          TotalAmount = b.TotalAmount,
                                          Remarks = b.Remarks
                                      })
                                      .Skip(startIndex)
                                      .Take(pageSize)
                                      .ToArrayAsync();

            return new PagedResult<BookingDisplayModel>(totalCount, bookings);
        }

        public async Task<MethodResult> ApproveBookingAsync(long bookingId)
        {
            using var context = _contextFactory.CreateDbContext();
            var booking = await context.Bookings
                                       .AsTracking()
                                       .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking is null)
                return "Недопустимый запрос";

            switch (booking.Status)
            {
                case Constants.BookingStatus.Booked:
                    return "Уже одобрено";
                case Constants.BookingStatus.Cancelled:
                    return "Бронирование отменено";
                case Constants.BookingStatus.PaymentSuccess:
                    booking.Status = Constants.BookingStatus.Booked;
                    break;
                default:
                    return "Бронирование может быть подтверждено только после оплаты";

            }
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<MethodResult> CancelBookingAsync(long bookingId, string cancelReason)
        {
            using var context = _contextFactory.CreateDbContext();
            var booking = await context.Bookings
                                       .AsTracking()
                                       .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking is null)
                return "Недопустимый запрос";

            if (booking.Status == Constants.BookingStatus.Cancelled)
                return "Уже отменен";

            booking.Status = Constants.BookingStatus.Cancelled;
            booking.Remarks += Environment.NewLine + $"Отменено персоналом/администратором. Причина: {cancelReason}";

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<BookingDisplayModel>> GetBookingsForGuestAsync(string guestId, BookingDisplayModel type, int startIndex, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Bookings;

            var totalCount = await query.CountAsync();

            var bookings = await query.OrderByDescending(b => b.CheckInDate)
                                      .Select(b => new BookingDisplayModel
                                      {
                                          Adults = b.Adults,
                                          BookedOn = b.BookedOn,
                                          CheckInDate = b.CheckInDate,
                                          CheckOutDate = b.CheckOutDate,
                                          Children = b.Children,
                                          GuestId = b.GuestId,
                                          GuestName = b.Guest.FullName,
                                          RoomTypeId = b.RoomTypeId,
                                          RoomTypeName = b.RoomType.Name,
                                          Id = b.Id,
                                          RoomId = b.RoomId,
                                          RoomNumber = b.RoomId == null ? "" : b.Room.RoomNumber,
                                          SpecialRequest = b.SpecialRequest,
                                          Status = b.Status,
                                          TotalAmount = b.TotalAmount,
                                          Remarks = b.Remarks
                                      })
                                      .Skip(startIndex)
                                      .Take(pageSize)
                                      .ToArrayAsync();

            return new PagedResult<BookingDisplayModel>(totalCount, bookings);
        }
    }
}
