using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LifeAdminServices
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext db;
        public TaskService(ApplicationDbContext db) => this.db = db;

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
            => await db.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Owner) 
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();

        public Task<TaskItem?> GetByIdAsync(int id)
            => db.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id);



        public async Task<IEnumerable<TaskItem>> GetMineAsync(string userId)
            => await db.TaskItems
                .Where(t => t.OwnerId == userId)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();

        public Task<TaskItem?> GetByIdOwnedAsync(int id, string userId)
            => db.TaskItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

        public async Task AddAsync(TaskItem task)
        {
            db.TaskItems.Add(task);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            db.TaskItems.Update(task);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            db.TaskItems.Remove(task);
            await db.SaveChangesAsync();
        }
        public async Task<bool> ExistsOwnedAsync(int id, string userId)
             => await db.TaskItems
            .AnyAsync(t => t.Id == id && t.OwnerId == userId);



    }
}
