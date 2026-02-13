using LifeAdminModels.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LifeAdminServices.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetAllForSelectAsync();
        Task<bool> ExistsAsync(int id);

        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
