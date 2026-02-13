using LifeAdminData;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace LifeAdminServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext db;
        public CategoryService(ApplicationDbContext db) => this.db = db;

        public async Task<IEnumerable<SelectListItem>> GetAllForSelectAsync()
            => await db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();


        public Task<bool> ExistsAsync(int id)
            => db.Categories.AnyAsync(c => c.Id == id);


    }
}
