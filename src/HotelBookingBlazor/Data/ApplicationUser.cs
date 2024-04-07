using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(10), RegularExpression(@"[a-zA-Z]+$"), Unicode(false)]
        public string FirstName { get; set; }

        [MaxLength(10), Unicode(false)]
        public string? LastName { get; set; }

        [Required, MaxLength(8), Unicode(false)]
        public string RoleName { get; set; }

        [Required, MaxLength(10), RegularExpression(@"^((\+7|7|8)+([0-9]){10})$")]
        public string ContactNumber { get; set; }
    }

}
