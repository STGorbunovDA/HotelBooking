using HotelBookingBlazor.Components.Pages;
using HotelBookingBlazor.Data;
using HotelBookingBlazor.Data.Entities;
using HotelBookingBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services
{
    public interface IRoomTypeService
    {
        Task<MethodResult<short>> SaveRoomTypeAsync(RoomTypeSaveModel model, string userId);
        Task<RoomTypeListModel[]> GetRoomTypesForManagePageAsync();
        Task<RoomTypeSaveModel?> GetRoomTypeToEditAsync(short id);
        Task<Room[]> GetRoomsAsync(short roomTypeId);
        Task<MethodResult<Room>> SaveRoomAsync(Room room);
        Task<MethodResult> DeleteRoomAsync(int roomId);
    }

    public class RoomTypeService : IRoomTypeService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public RoomTypeService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MethodResult<short>> SaveRoomTypeAsync(RoomTypeSaveModel model, string userId)
        {
            using var context = _contextFactory.CreateDbContext();

            RoomType? roomType;

            if (model.Id == 0)
            {
                if (await context.RoomTypes.AnyAsync(rt => rt.Name == model.Name))
                {
                    return $"Номер с таким же названием {model.Name} уже существует";
                }

                roomType = new RoomType
                {
                    Name = model.Name,
                    AddedBy = userId,
                    AddedOn = DateTime.Now,
                    Description = model.Description,
                    Image = model.Image,
                    IsActive = model.IsActive,
                    MaxAdults = model.MaxAdults,
                    MaxChildren = model.MaxChildren,
                    Price = model.Price,
                };

                await context.RoomTypes.AddAsync(roomType);
            }
            else
            {
                if (await context.RoomTypes.AnyAsync(rt => rt.Name == model.Name && rt.Id != model.Id))
                {
                    return $"Номер с таким же названием {model.Name} уже существует";
                }

                roomType = await context.RoomTypes
                                              .AsTracking()
                                              .FirstOrDefaultAsync(rt => rt.Id == model.Id);

                if (roomType is null)
                {
                    return $"Недопустимый запрос";
                }

                roomType.Name = model.Name;
                roomType.Description = model.Description;
                roomType.Image = model.Image;
                roomType.IsActive = model.IsActive;
                roomType.MaxAdults = model.MaxAdults;
                roomType.MaxChildren = model.MaxChildren;
                roomType.Price = model.Price;

                roomType.LastUpdatedBy = userId;
                roomType.LastUpdatedOn = DateTime.Now;

                var existingRoomTypeAmenities = await context.RoomTypeAmenities
                                                .Where(rta => rta.RoomTypeId == model.Id)
                                                .ToListAsync();
                context.RoomTypeAmenities.RemoveRange(existingRoomTypeAmenities);
            }

            await context.SaveChangesAsync();

            if (model.Amenities.Length > 0)
            {
                var roomTypeAmentities = model.Amenities.Select(a => new RoomTypeAmenity
                {
                    AmenityId = a.Id,
                    RoomTypeId = roomType.Id,
                    Unit = a.Unit,
                });
                await context.RoomTypeAmenities.AddRangeAsync(roomTypeAmentities);
                await context.SaveChangesAsync();
            }

            return roomType.Id;
        }

        public async Task<RoomTypeListModel[]> GetRoomTypesForManagePageAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.RoomTypes
                                .Select(rt => new RoomTypeListModel(rt.Id, rt.Name, rt.Image, rt.Price))
                                .ToArrayAsync();
        }

        public async Task<RoomTypeSaveModel?> GetRoomTypeToEditAsync(short id)
        {
            using var context = _contextFactory.CreateDbContext();
            var roomType = await context.RoomTypes
                                  .Include(rt => rt.RoomTypeAmenities)
                                  .Where(rt => rt.Id == id)
                                  .Select(rt => new RoomTypeSaveModel
                                  {
                                      Name = rt.Name,
                                      Image = rt.Image,
                                      Price = rt.Price,
                                      Description = rt.Description,
                                      IsActive = rt.IsActive,
                                      Id = rt.Id,
                                      MaxAdults = rt.MaxAdults,
                                      MaxChildren = rt.MaxChildren,
                                      Amenities = rt.RoomTypeAmenities.Select(
                                                        a => new RoomTypeSaveModel
                                                                .RoomTypeAmenitySaveModel(a.AmenityId, a.Unit)).ToArray()
                                  }).FirstOrDefaultAsync();

            return roomType;
        }

        public async Task<Room[]> GetRoomsAsync(short roomTypeId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Rooms
                                .Where(r => r.RoomTypeId == roomTypeId && !r.IsDeleted)
                                .ToArrayAsync();
        }

        public async Task<MethodResult<Room>> SaveRoomAsync(Room room)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                if (room.Id == 0)
                {
                   await context.Rooms.AddAsync(room);
                }
                else
                {
                    var dbRoom = await context.Rooms
                                              .AsTracking()
                                              .FirstOrDefaultAsync(r => r.Id == room.Id && !r.IsDeleted);
                    if (dbRoom is null)
                    {
                        return "Invalid request";
                    }
                    dbRoom.IsAvailable = room.IsAvailable;
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return room;
        }

        public async Task<MethodResult> DeleteRoomAsync(int roomId)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbRoom = await context.Rooms
                                            .AsTracking()
                                            .FirstOrDefaultAsync(r => r.Id == roomId);
            if (dbRoom is null)
            {
                return "Invalid request";
            }

            dbRoom.IsDeleted = true;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
