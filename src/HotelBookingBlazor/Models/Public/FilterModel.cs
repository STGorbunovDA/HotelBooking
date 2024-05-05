namespace HotelBookingBlazor.Models.Public
{
    public class FilterModel
    {
        public FilterModel() { }

        public static FilterModel GetFilterModel(DateOnly? checkInDate, DateOnly? checkOutDate, int? adults, int? children)
        {
            return new()
            {
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                Adults = adults,
                Children = children
            };
        }

        public DateOnly? CheckInDate { get; set; } // = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly? CheckOutDate { get; set; } // = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        public int? Adults { get; set; } = 0;
        public int? Children { get; set; } = 0;

        public IReadOnlyDictionary<string, object?> ToDictionary() =>
            new Dictionary<string, object?>
            {
                [nameof(CheckInDate)] = CheckInDate,
                [nameof(CheckOutDate)] = CheckOutDate,
                [nameof(Adults)] = Adults,
                [nameof(Children)] = Children,
            };
    }
}
