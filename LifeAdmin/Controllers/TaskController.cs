using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using LifeAdmin.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService tasks;
        private readonly ICategoryService categories;
        private readonly UserManager<ApplicationUser> userManager;
        private bool IsAdmin => User.IsInRole("Admin");


        public TasksController(
            ITaskService tasks,
            ICategoryService categories,
            UserManager<ApplicationUser> userManager)
        {
            this.tasks = tasks;
            this.categories = categories;
            this.userManager = userManager;
        }

        private string UserId => userManager.GetUserId(User)!;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var all = await tasks.GetAllAsync();
                        
            var users = userManager.Users
                .Select(u => new { u.Id, u.Email, u.UserName })
                .ToList()
                .ToDictionary(u => u.Id, u => u);

            var model = all.Select(t =>
            {
                users.TryGetValue(t.OwnerId, out var u);

                return new TaskListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Status = t.Status,
                    CreatedOn = t.CreatedOn,
                    OwnerEmail = u?.Email,
                    OwnerUserName = u?.UserName
                };
            });

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> All()
        {
            var mine = await tasks.GetMineAsync(UserId);

            var model = mine
                .Select(t => new TaskListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Status = t.Status,
                    CreatedOn = t.CreatedOn
                })
                .ToList();

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
        [ValidateAntiForgeryToken]
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
                OwnerId = UserId,
                CreatedOn = DateTime.UtcNow
            };

            await tasks.AddAsync(entity);
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = IsAdmin
            ? await tasks.GetByIdAsync(id)
            : await tasks.GetByIdOwnedAsync(id, UserId);
            
            if (task is null) return NotFound();

            var vm = new TaskDetailsViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CategoryName = task.Category.Name,
                Status = task.Status,
                CreatedOn = task.CreatedOn
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = IsAdmin
            ? await tasks.GetByIdAsync(id)
            : await tasks.GetByIdOwnedAsync(id, UserId);
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
        [ValidateAntiForgeryToken]
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
            var task = IsAdmin
             ? await tasks.GetByIdAsync(id)
             : await tasks.GetByIdOwnedAsync(id, UserId);

            if (task is null) return NotFound();

            var vm = new TaskDeleteViewModel
            {
                Id = task.Id,
                Title = task.Title,
                CategoryName = task.Category.Name,
                Status = task.Status
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var task = await tasks.GetByIdOwnedAsync(id, UserId);
            if (task is null) return NotFound();

            await tasks.DeleteAsync(task);
            return RedirectToAction(nameof(All));
        }

    }
}
