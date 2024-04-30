using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models
{
    public class EditStaffModel
    {
        public string Id { get; set; }

        [Required, MaxLength(10), RegularExpression(@"[а-яА-Я]+$")]
        public string FirstName { get; set; } = "";

        [MaxLength(10), RegularExpression(@"[а-яА-Я]+$")]
        public string? LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required, MaxLength(15), RegularExpression(@"^((\+7|7|8)+([0-9]){10})$")]
        public string ContactNumber { get; set; } = "";

        [MaxLength(50)]
        public string? Designation { get; set; }
    }
}
