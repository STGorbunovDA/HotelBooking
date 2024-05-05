using HotelBookingBlazor.Data;
using HotelBookingBlazor.Models.Public;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services.Public
{
    public interface IRoomsService
    {
        Task<RoomTypeModel[]> GetRoomTypesAsync(int count = 0, FilterModel filter = null);
    }

    public class RoomsService : IRoomsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public RoomsService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<RoomTypeModel[]> GetRoomTypesAsync(int count = 0, FilterModel filter = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomTypes
                            .Where(rt => rt.IsActive);
            if(filter is not null)
            {
                if(filter.Adults > 0)
                {
                    query = query.Where(rt => rt.MaxAdults >= filter.Adults);
                }
                if (filter.Children > 0)
                {
                    query = query.Where(rt => rt.MaxChildren >= filter.Children);
                }

                // get the booking for these checkin checkout dates
                // check the available room types for those dates
            }

            if (count > 0)
            {
                query = query.Take(count);
            }
            return await query.Select(rt =>
                                new RoomTypeModel(
                                    rt.Id,
                                    rt.Name,
                                    rt.Image,
                                    rt.Description!,
                                    rt.Price,
                                    rt.RoomTypeAmenities.Select(a =>
                                        new RoomTypeAmenityModel(
                                            a.Amenity.Name,
                                            a.Amenity.Icon,
                                            a.Unit)).ToArray())).ToArrayAsync();
        }
    }
}
