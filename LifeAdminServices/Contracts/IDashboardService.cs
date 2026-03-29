using ViewModels.DashboardViewModels;

namespace LifeAdminServices.Contracts
{
    public interface IDashboardService
    {
        Task<DashboardStatsViewModel> GetStatsAsync(string userId);
    }
}
