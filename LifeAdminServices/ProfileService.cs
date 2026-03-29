using LifeAdminData;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using ViewModels.ProfilesViewModels;

namespace LifeAdminServices
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext db;

        public ProfileService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            return await db.Users
                .Where(u => u.Id == userId)
                .Select(u => new ProfileViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    DisplayName = u.DisplayName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfileImageUrl = u.ProfileImageUrl,
                    Bio = u.Bio,
                    PhoneNumber = u.PhoneNumber,
                    CreatedOn = u.CreatedOn
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EditProfileViewModel?> GetProfileForEditAsync(string userId)
        {
            return await db.Users
                .Where(u => u.Id == userId)
                .Select(u => new EditProfileViewModel
                {
                    DisplayName = u.DisplayName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfileImageUrl = u.ProfileImageUrl,
                    Bio = u.Bio,
                    PhoneNumber = u.PhoneNumber
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProfileAsync(string userId, EditProfileViewModel model)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            user.DisplayName = model.DisplayName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.ProfileImageUrl = model.ProfileImageUrl;
            user.Bio = model.Bio;
            user.PhoneNumber = model.PhoneNumber;

            await db.SaveChangesAsync();
            return true;
        }
    }
}