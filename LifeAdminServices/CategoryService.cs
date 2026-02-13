using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


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

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await db.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

        public Task<Category?> GetByIdAsync(int id)
            => db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category category)
        {
            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            db.Categories.Update(category);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
        }


    }
}
