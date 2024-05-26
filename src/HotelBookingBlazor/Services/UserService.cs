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
        Task<PagedResult<UserDisplayModel>> GetUserAsync(int startIndex, int pageSize, RoleType? roleType = null);
        Task<MyProfileModel?> GetProfileDetailsAsync(string userId);
        Task<MethodResult> UpdateProfileAsync(MyProfileModel model, RoleType? roleType = null);
        Task<MethodResult> ChangePasswordAsync(ChangePasswordModel model, string userId);
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

        public async Task<PagedResult<UserDisplayModel>> GetUserAsync(int startIndex, int pageSize, RoleType? roleType = null)
        {
            var query = _userManager.Users;
            if (roleType is not null)
            {
                query = query.Where(u => u.RoleName == roleType.ToString());
            }

            var total = await query.CountAsync();

            var records = await query.Select(u => new UserDisplayModel(u.Id, u.FullName, u.Email,
                                      u.RoleName, u.ContactNumber, u.Designation))
                              .Skip(startIndex)
                              .Take(pageSize)
                              .ToArrayAsync();
            return new PagedResult<UserDisplayModel>(total, records);
        }

        public async Task<MethodResult<ApplicationUser>> CreateUserAsync(ApplicationUser user,
            string email, string password)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser is not null)
            {
                return new MethodResult<ApplicationUser>(false, "Email exists already", existingUser);
            }

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

        public async Task<MyProfileModel?> GetProfileDetailsAsync(string userId) =>
                     await GetUser(userId)
                            .Select(u => new MyProfileModel
                            {
                                Id = u.Id,
                                ContactNumber = u.ContactNumber,
                                Designation = u.Designation,
                                Email = u.Email,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                            }).FirstOrDefaultAsync();

        private IQueryable<ApplicationUser> GetUser(string userId, RoleType? roleType = null)
        {
            var query = _userManager.Users
                        .Where(u => u.Id == userId);
            if (roleType is not null)
            {
                query = query.Where(u => u.RoleName == RoleType.Staff.ToString());
            }
            return query;
        }

        public async Task<MethodResult> UpdateProfileAsync(MyProfileModel model, RoleType? roleType = null)
        {
            var dbUser = await GetUser(model.Id, roleType)
                               .FirstOrDefaultAsync();
            if (dbUser is null)
            {
                return "Invalid request";
            }

            dbUser.UserName = model.Email; // костыль
            dbUser.FirstName = model.FirstName;
            dbUser.LastName = model.LastName;
            dbUser.Designation = model.Designation;
            dbUser.ContactNumber = model.ContactNumber;

            var result = await _userManager.UpdateAsync(dbUser);

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }

            if (dbUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Если Администратор не менял адрес электронной почты,
                // то возвращаемся
                return true;
            }

            // Администратор изменил электронный адрес сотрудника

            var changeToken = await _userManager.GenerateChangeEmailTokenAsync(dbUser, model.Email);

            result = await _userManager.ChangeEmailAsync(dbUser, model.Email, changeToken);

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }

            return true;
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Пользовательский интерфейс по умолчанию требует наличия пользовательского хранилища с поддержкой электронной почты.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public async Task<MethodResult> ChangePasswordAsync(ChangePasswordModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return "Invalid request";

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }
            return true;
        }
    }
}
