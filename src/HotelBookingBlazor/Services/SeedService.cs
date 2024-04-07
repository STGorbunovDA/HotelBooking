using HotelBookingBlazor.Constants;
using HotelBookingBlazor.Data;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingBlazor.Services
{
    public class SeedService
    {
        public UserManager<ApplicationUser> _userManager { get; }
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public SeedService(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore,
                           RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task SeedDatabaseAsync()
        {
            var adminUserEmail = _configuration.GetValue<string>("AdminUser:Email");
            var dbAdminUser = _userManager.FindByEmailAsync(email: adminUserEmail);
            if (dbAdminUser is not null) return; // Если админ есть в БД

            var applicationUser = new ApplicationUser()
            {
                FirstName = _configuration.GetValue<string>("AdminUser:FirstName")!,
                LastName = _configuration.GetValue<string>("AdminUser:LastName"),
                RoleName = RoleType.Admin.ToString(),
                ContactNumber = _configuration.GetValue<string>("AdminUser:ContactNumber")!,
                Designation = "Administrator",
                Email = adminUserEmail,
            };
            await _userStore.SetUserNameAsync(applicationUser, adminUserEmail, default);

            var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
            await emailStore.SetUserNameAsync(applicationUser, adminUserEmail, default);

            var result = await _userManager.CreateAsync(applicationUser,
                _configuration.GetValue<string>("AdminUser:Password")!);

            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                throw new Exception($"Error is creating user.\n{errors}");
            }

            if (await _roleManager.FindByNameAsync(RoleType.Admin.ToString()) is null)
            {
                foreach (var roleName in Enum.GetNames<RoleType>())
                {
                    var role = new IdentityRole
                    {
                        Name = roleName,
                    };
                    await _roleManager.CreateAsync(role);
                }
            }
            result = await _userManager.AddToRoleAsync(applicationUser, RoleType.Admin.ToString());

            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                throw new Exception($"Error in adding to Admin role.\n{errors}");
            }
        }
    }
}
