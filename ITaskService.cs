using LifeAdmin.Data.Models.Entities;

namespace LifeAdmin.Services.Core.Contracts
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetMineAsync(string userId);
        Task<TaskItem?> GetByIdOwnedAsync(int id, string userId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
    }
}
