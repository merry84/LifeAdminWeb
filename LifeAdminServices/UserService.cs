using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewModels.Admin;

namespace LifeAdminServices
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var users = await db.Users.ToListAsync();

            var result = new List<UserViewModel>();

            foreach (var user in users)
            {
                var isAdmin = await userManager.IsInRoleAsync(user, "Administrator");

                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedOn = user.CreatedOn,
                    IsAdmin = isAdmin
                });
            }

            return result;
        }

        public async Task<bool> ToggleAdminRoleAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (await userManager.IsInRoleAsync(user, "Administrator"))
            {
                await userManager.RemoveFromRoleAsync(user, "Administrator");
            }
            else
            {
                await userManager.AddToRoleAsync(user, "Administrator");
            }

            return true;
        }
    }
}