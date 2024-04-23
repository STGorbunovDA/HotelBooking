using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models
{
    public class RegisterUserModel
    {
        [Required, MaxLength(15), RegularExpression(@"[a-zA-Z]+$")]
        public string FirstName { get; set; } = "";

        [MaxLength(15), RegularExpression(@"[a-zA-Z]+$")]
        public string? LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required, MaxLength(15), RegularExpression(@"^((\+7|7|8)+([0-9]){10})$")]
        public string ContactNumber { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
