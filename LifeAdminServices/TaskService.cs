using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using ViewModels;

namespace LifeAdminServices
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext db;

        public TaskService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
            => await db.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetAllAsync(string userId)
            => await db.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.OwnerId == userId)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetMineAsync(string userId)
            => await db.TaskItems
                .AsNoTracking()
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.OwnerId == userId)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(Guid id)
            => await db.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TaskItem?> GetByIdOwnedAsync(Guid id, string userId)
            => await db.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

        public async Task<IEnumerable<TaskItem>> GetDeletedAsync()
            => await db.TaskItems
                .IgnoreQueryFilters()
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.IsDeleted)
                .OrderByDescending(t => t.DeletedOn)
                .ToListAsync();

        public async Task<TaskItem?> GetDeletedByIdAsync(Guid id)
            => await db.TaskItems
                .IgnoreQueryFilters()
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsDeleted);

        public async Task RestoreAsync(TaskItem task)
        {
            task.IsDeleted = false;
            task.DeletedOn = null;
            await db.SaveChangesAsync();
        }

        public async Task AddAsync(TaskItem task)
        {
            await db.TaskItems.AddAsync(task);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            db.TaskItems.Update(task);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            task.IsDeleted = true;
            task.DeletedOn = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        public async Task<bool> ExistsOwnedAsync(Guid id, string userId)
            => await db.TaskItems
                .AnyAsync(t => t.Id == id && t.OwnerId == userId);

        public async Task<TaskQueryViewModel> GetAllAsync(
            string userId,
            string? searchTerm,
            Guid? categoryId,
            int? status,
            int currentPage,
            int tasksPerPage)
        {
            var tasksQuery = db.TaskItems
                .AsNoTracking()
                .Include(t => t.Category)
                .Include(t => t.Owner)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string normalizedSearch = searchTerm.ToLower();

                tasksQuery = tasksQuery.Where(t =>
                    t.Title.ToLower().Contains(normalizedSearch));
            }

            if (categoryId.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => (int)t.Status == status.Value);
            }

            int totalTasks = await tasksQuery.CountAsync();

            var tasks = await tasksQuery
                .OrderByDescending(t => t.CreatedOn)
                .Skip((currentPage - 1) * tasksPerPage)
                .Take(tasksPerPage)
                .Select(t => new TaskListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Status = t.Status,
                    CreatedOn = t.CreatedOn,
                    OwnerUserName = t.Owner.UserName,
                    OwnerEmail = t.Owner.Email,
                    Tags = t.TaskItemTags
                        .Select(tt => tt.Tag.Name)
                        .ToList()
                })
                .ToListAsync();

            var categories = await db.Categories
                .AsNoTracking()
                .Select(c => new TaskCategoryOptionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return new TaskQueryViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Status = status,
                CurrentPage = currentPage,
                TasksPerPage = tasksPerPage,
                TotalTasksCount = totalTasks,
                Tasks = tasks,
                Categories = categories
            };
        }
    }
}