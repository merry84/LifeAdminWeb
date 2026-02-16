using LifeAdminData;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using ViewModels;

namespace LifeAdminServices
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext db;

        public DashboardService(ApplicationDbContext db) => this.db = db;

        public async Task<DashboardStatsViewModel> GetStatsAsync(string userId)
        {
            return new DashboardStatsViewModel
            {
                MyTasksCount = await db.TaskItems.CountAsync(t => t.OwnerId == userId),
                MyNotesCount = await db.Notes.CountAsync(n => n.OwnerId == userId),
                CategoriesCount = await db.Categories.CountAsync()
            };
        }
    }
}
