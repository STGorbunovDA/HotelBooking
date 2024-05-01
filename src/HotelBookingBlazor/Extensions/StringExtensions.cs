namespace HotelBookingBlazor.Extensions
{
    public static class StringExtensions
    {
        public static string Ellipsis(this string str, int maxLenght) =>
            (string.IsNullOrWhiteSpace(str) || str.Length <= maxLenght)
            ? str
            : $"{str[0..97]}...";

    }
}
