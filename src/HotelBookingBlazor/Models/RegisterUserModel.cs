using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models
{
    public class RegisterUserModel
    {
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

        [Required]
        [StringLength(100, ErrorMessage = "Длина символа {0} должна составлять не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = "";

        [MaxLength(50)]
        public string? Designation { get; set; }
    }
}
