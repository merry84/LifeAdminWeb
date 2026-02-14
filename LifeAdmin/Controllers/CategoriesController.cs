using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categories;
        public CategoriesController(ICategoryService categories) => this.categories = categories;

        public async Task<IActionResult> All()
            => View(await categories.GetAllAsync());

        [HttpGet]
        public IActionResult Create() => View(new Category());

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid) return View(model);
            await categories.AddAsync(model);
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cat = await categories.GetByIdAsync(id);
            if (cat is null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            var cat = await categories.GetByIdAsync(model.Id);
            if (cat is null) return NotFound();

            cat.Name = model.Name;
            await categories.UpdateAsync(cat);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await categories.GetByIdAsync(id);
            if (cat is null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cat = await categories.GetByIdAsync(id);
            if (cat is null) return NotFound();

            await categories.DeleteAsync(cat);
            return RedirectToAction(nameof(All));
        }
    }
}
