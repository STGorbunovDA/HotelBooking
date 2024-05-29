namespace HotelBookingBlazor.Models
{
    public class ContactInfoOptions
    {
        public const string Key = "ContactInfo"; // Key from AppSettings.json
        public string ContactNumber { get; set; }
        public string Adress { get; set; }
        public string GeneralEmail { get; set; }
        public string TechnicalEmail { get; set; }
        public string BookingEmail { get; set; }
    }
}
