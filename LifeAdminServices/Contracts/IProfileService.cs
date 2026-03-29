

using ViewModels;
using ViewModels.Profile;

namespace LifeAdminServices.Contracts
{
    public interface IProfileService
    {
        Task<ProfileViewModel?> GetProfileAsync(string userId);
        Task<EditProfileViewModel?> GetProfileForEditAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, EditProfileViewModel model);
    }
}