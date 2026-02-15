using LifeAdmin.Web.Infrastructure;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace LifeAdmin.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categories;

        public CategoriesController(ICategoryService categories)
            => this.categories = categories;

        
        public async Task<IActionResult> All()
        {
            var list = await categories.GetAllAsync();

            var model = list
                .OrderBy(c => c.Name)
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
            => View(new CategoryFormViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var entity = new Category { Name = vm.Name.Trim() };
            await categories.AddAsync(entity);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var c = await categories.GetByIdAsync(id);
            if (c is null) return NotFound();

            var vm = new CategoryFormViewModel { Id = c.Id, Name = c.Name };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryFormViewModel vm)
        {
            var c = await categories.GetByIdAsync(vm.Id);
            if (c is null) return NotFound();

            if (!ModelState.IsValid) return View(vm);

            c.Name = vm.Name.Trim();
            await categories.UpdateAsync(c);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await categories.GetByIdAsync(id);
            if (c is null) return NotFound();

            var vm = new CategoryFormViewModel { Id = c.Id, Name = c.Name };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var c = await categories.GetByIdAsync(id);
            if (c is null) return NotFound();

            await categories.DeleteAsync(c);
            return RedirectToAction(nameof(All));
        }
    }
}
