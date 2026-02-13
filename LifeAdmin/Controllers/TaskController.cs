using System.Security.Claims;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;


namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService tasks;
        private readonly ICategoryService categories;

        public TasksController(ITaskService tasks, ICategoryService categories)
        {
            this.tasks = tasks;
            this.categories = categories;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> All()
        {
            var mine = await tasks.GetMineAsync(UserId);

            var model = mine.Select(t => new TaskListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                CategoryName = t.Category.Name,
                Status = t.Status,
                CreatedOn = t.CreatedOn
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new TaskFormViewModel
            {
                Categories = await categories.GetAllForSelectAsync(),
                Status = WorkStatus.Planned
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskFormViewModel vm)
        {
            if (!await categories.ExistsAsync(vm.CategoryId))
                ModelState.AddModelError(nameof(vm.CategoryId), "Invalid category.");

            if (!ModelState.IsValid)
            {
                vm.Categories = await categories.GetAllForSelectAsync();
                return View(vm);
            }

            var entity = new TaskItem
            {
                Title = vm.Title,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                Status = vm.Status,
                OwnerId = UserId
            };

            await tasks.AddAsync(entity);
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = await tasks.GetByIdOwnedAsync(id, UserId);
            if (task is null) return NotFound();

            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await tasks.GetByIdOwnedAsync(id, UserId);
            if (task is null) return NotFound();

            var vm = new TaskFormViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CategoryId = task.CategoryId,
                Status = task.Status,
                Categories = await categories.GetAllForSelectAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskFormViewModel vm)
        {
            var task = await tasks.GetByIdOwnedAsync(vm.Id, UserId);
            if (task is null) return NotFound();

            if (!await categories.ExistsAsync(vm.CategoryId))
                ModelState.AddModelError(nameof(vm.CategoryId), "Invalid category.");

            if (!ModelState.IsValid)
            {
                vm.Categories = await categories.GetAllForSelectAsync();
                return View(vm);
            }

            task.Title = vm.Title;
            task.Description = vm.Description;
            task.CategoryId = vm.CategoryId;
            task.Status = vm.Status;

            await tasks.UpdateAsync(task);
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await tasks.GetByIdOwnedAsync(id, UserId);
            if (task is null) return NotFound();

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await tasks.GetByIdOwnedAsync(id, UserId);
            if (task is null) return NotFound();

            await tasks.DeleteAsync(task);
            return RedirectToAction(nameof(All));
        }
    }
}
