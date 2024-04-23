using HotelBookingBlazor.Constants;
using HotelBookingBlazor.Data;
using HotelBookingBlazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services
{
    public interface IUserService
    {
        Task<MethodResult<ApplicationUser>> CreateUserAsync(ApplicationUser user, string email, string password);
        Task<UserDisplayModel[]> GetUserAsync(RoleType? roleType = null);
    }

    public class UserService : IUserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UserService(IDbContextFactory<ApplicationDbContext> contextFactory,
                            UserManager<ApplicationUser> userManager,
                            IUserStore<ApplicationUser> userStore)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _userStore = userStore;
        }

        public async Task<UserDisplayModel[]> GetUserAsync(RoleType? roleType = null)
        {
            var query = _userManager.Users;
            if(roleType is not null)
            {
                query = query.Where(u => u.RoleName == roleType.ToString());
            }
            return await query.Select(u => new UserDisplayModel(u.Id, u.FullName, u.Email, 
                                      u.RoleName, u.ContactNumber, u.Designation))
                               .ToArrayAsync();
        }

        public async Task<MethodResult<ApplicationUser>> CreateUserAsync(ApplicationUser user,
            string email, string password)
        {
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }

            result = await _userManager.AddToRoleAsync(user, user.RoleName ?? RoleType.Guest.ToString());

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }
            return user;
        }

        //public async Task<MethodResult> UpdateUserAsync(ApplicationUser user)
        //{

        //}

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
