using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Models.Public
{
    public class BookingModel
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

        public DateOnly CheckInDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly CheckOutDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        public int? Adults { get; set; } = 0;
        public int? Children { get; set; } = 0;

        [Range(1, 100, ErrorMessage = "Please select room")]
        public short RoomTypeId { get; set; }

        [MaxLength(150)]
        public string? SpecialRequest { get; set; }

        public decimal Amount { get; set; }

        public void SetDummyValues()
        {
            // используем это только для обхода проверки DataAnnotation
            Email = "blazing@blazing.com";
            ContactNumber = "1234567890";

            FirstName = "Blazing";
            LastName = "Hotel";

            Password = ConfirmPassword = "@Dmitriy123";
        }

        //public void SetValuesFromClaimsPrincipal(ClaimsPrincipal principal)
        //{
        //    if (principal?.Identity?.IsAuthenticated == true)
        //    {
        //        // var userId = principal.GetUserId();
        //        // var roleName = principal.FindFirstValue(AppConstans.CustomClaimTypes.RoleName);

        //        var fullName = principal.FindFirstValue(AppConstans.CustomClaimTypes.FullName);
        //        Email = principal.FindFirstValue(AppConstans.CustomClaimTypes.Email)!;
        //        ContactNumber = principal.FindFirstValue(AppConstans.CustomClaimTypes.ContactNumber)!;

        //        var parts = fullName.Split(' ');
        //        FirstName = parts[0];
        //        LastName = parts.Length > 1 ? parts[1] : null;

        //        // используем это только для обхода проверки DataAnnotation
        //        Password = ConfirmPassword = "@Dmitriy123";
        //    }

        //}
    }
}
