using LifeAdminModels.Models;

namespace LifeAdminServices.Contracts
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetMineAsync(string userId);
        Task<TaskItem?> GetByIdOwnedAsync(int id, string userId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<bool> ExistsOwnedAsync(int id, string userId);
    }
}
