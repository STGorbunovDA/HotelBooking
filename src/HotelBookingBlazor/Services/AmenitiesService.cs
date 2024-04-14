using HotelBookingBlazor.Data;
using HotelBookingBlazor.Data.Entities;
using HotelBookingBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services
{
    public interface IAmenitiesService
    {
        Task<Amenity[]> GetAmenitiesAsync();
        Task<MethodResult<Amenity>> SaveAmenityAsync(Amenity amenity);
        Task<MethodResult<bool>> DeleteAmenityAsync(int id);
    }

    public class AmenitiesService : IAmenitiesService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public AmenitiesService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Amenity[]> GetAmenitiesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Amenities.Where(a => !a.IsDeleted).ToArrayAsync();
        }

        public async Task<MethodResult<Amenity>> SaveAmenityAsync(Amenity amenity)
        {
            using var context = _contextFactory.CreateDbContext();
            if (amenity.Id == 0)
            {
                // Create new Amenity

                if (await context.Amenities.AnyAsync(a => a.Name == amenity.Name && !a.IsDeleted))
                {
                    //return MethodResult<Amenity>.Failure("Удобства уже существуют");
                    return "Такие удобства уже существуют";
                }

                await context.Amenities.AddAsync(amenity);

            }
            else
            {
                if (await context.Amenities.AnyAsync(a => a.Name == amenity.Name && a.Id != amenity.Id && !a.IsDeleted))
                {
                    return "Такие удобства уже существуют";
                }


                // Update existing Amenity
                var dbAmenity = await context.Amenities
                                                .AsTracking()
                                                .FirstOrDefaultAsync(a => a.Id == amenity.Id)
                                ?? throw new InvalidOperationException("Amenity does not exist");
                dbAmenity.Name = amenity.Name;
                dbAmenity.Icon = amenity.Icon;
            }

            await context.SaveChangesAsync();
            return amenity;
        }

        public async Task<MethodResult<bool>> DeleteAmenityAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var amenity = await context.Amenities.AsTracking().FirstOrDefaultAsync(a => a.Id == id);

            if(amenity is not null)
            {
                amenity.IsDeleted = true;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }

    public class RoomService
    {

    }
}
