using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models
{
    public class SubscribeModel
    {
        [Required, MaxLength(50), EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
