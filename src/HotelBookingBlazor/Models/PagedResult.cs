namespace HotelBookingBlazor.Models
{
    public record PagedResult<TData>(int TotalCount, TData[] Records);
}
