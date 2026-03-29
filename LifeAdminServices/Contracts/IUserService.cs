using ViewModels.Admin;

namespace LifeAdminServices.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllAsync();
        Task<bool> ToggleAdminRoleAsync(string userId);
    }
}