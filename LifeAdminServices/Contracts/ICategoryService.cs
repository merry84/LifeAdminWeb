using Microsoft.AspNetCore.Mvc.Rendering;

namespace LifeAdminServices.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetAllForSelectAsync();
        Task<bool> ExistsAsync(int id);
    }
}
