using System.Security.Claims;

namespace HotelBookingBlazor.Extensions
{
    public static class ClaimsPricipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
