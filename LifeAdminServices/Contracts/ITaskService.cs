using LifeAdminModels.Models;
using ViewModels;

namespace LifeAdminServices.Contracts
{
    public interface ITaskService
    {
        Task<TaskQueryViewModel> GetAllAsync(
               string userId,
               string? searchTerm,
               int? categoryId,
               int? status,
               int currentPage,
               int tasksPerPage);

        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);

        Task<IEnumerable<TaskItem>> GetMineAsync(string userId);
        Task<TaskItem?> GetByIdOwnedAsync(int id, string userId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<bool> ExistsOwnedAsync(int id, string userId);
    }
}
