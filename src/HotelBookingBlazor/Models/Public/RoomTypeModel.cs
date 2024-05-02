using HotelBookingBlazor.Data.Entities;

namespace HotelBookingBlazor.Models.Public
{
    public readonly record struct RoomTypeAmenityModel(string Name, string? Icon = null, int? Unit = null);
    public record RoomTypeModel(short Id, string Name, string Image, string Description, decimal Price, RoomTypeAmenityModel[] Amenities);
}
