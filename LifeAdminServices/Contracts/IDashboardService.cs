using ViewModels;

namespace LifeAdminServices.Contracts
{
    public interface IDashboardService
    {
        Task<DashboardStatsViewModel> GetStatsAsync(string userId);
    }
}
