using HotelBookingBlazor.Data;
using HotelBookingBlazor.Models.Public;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services.Public
{
    public interface IRoomsService
    {
        Task<RoomTypeModel[]> GetRoomTypesAsync();
    }

    public class RoomsService : IRoomsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public RoomsService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<RoomTypeModel[]> GetRoomTypesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var roomTypes = await context.RoomTypes
                            .Where(rt => rt.IsActive)
                            .Select(rt =>
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
                                            a.Unit)).ToArray())
                                ).ToArrayAsync();
            return roomTypes;
        }
    }
}
