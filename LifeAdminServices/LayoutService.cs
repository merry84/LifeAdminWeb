using LifeAdminData;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LifeAdminServices
{
    public class LayoutService : ILayoutService
    {
        private readonly ApplicationDbContext db;

        public LayoutService(ApplicationDbContext db) => this.db = db;

        public Task<int> GetMyTasksCountAsync(string userId)
            => db.TaskItems.CountAsync(t => t.OwnerId == userId);
    }
}
