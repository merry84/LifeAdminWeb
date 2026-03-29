using LifeAdminModels.Models;
using ViewModels.TasksViewModels;

namespace LifeAdminServices.Contracts
{
    public interface ITaskService
    {
        Task<TaskQueryViewModel> GetAllAsync(
            string userId,
            string? searchTerm,
            Guid? categoryId,
            int? status,
            int currentPage,
            int tasksPerPage);

        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<IEnumerable<TaskItem>> GetAllAsync(string userId);
        Task<IEnumerable<TaskItem>> GetMineAsync(string userId);
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem?> GetByIdOwnedAsync(Guid id, string userId);

        Task<IEnumerable<TaskItem>> GetDeletedAsync();
        Task<TaskItem?> GetDeletedByIdAsync(Guid id);
        Task RestoreAsync(TaskItem task);

        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<bool> ExistsOwnedAsync(Guid id, string userId);
    }
}