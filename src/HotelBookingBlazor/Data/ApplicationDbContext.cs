using HotelBookingBlazor.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<RoomTypeAmenity> RoomTypeAmenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<RoomTypeAmenity>()
                .HasKey(ra => new { ra.RoomTypeId, ra.AmenityId });

            builder.Entity<RoomType>() 
                .HasMany(rt => rt.Rooms) // у номера один тип 
                .WithOne(r => r.RoomType) // у типа много номеров
                .OnDelete(DeleteBehavior.NoAction); // при удалении типа комната остаётся.

        }
    }
}
