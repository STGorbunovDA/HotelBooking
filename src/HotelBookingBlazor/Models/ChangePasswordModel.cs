using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Длина символа {0} должна составлять не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Длина символа {0} должна составлять не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmNewPassword { get; set; } = "";
    }
}
