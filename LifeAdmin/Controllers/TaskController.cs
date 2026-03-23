using LifeAdmin.Web.Infrastructure;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using static GCommon.NotificationMessages;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService tasks;
        private readonly ICategoryService categories;
        private readonly UserManager<ApplicationUser> userManager;

        private bool IsAdmin => User.IsInRole("Administrator");
        private string UserId => userManager.GetUserId(User)!;

        public TasksController(
            ITaskService tasks,
            ICategoryService categories,
            UserManager<ApplicationUser> userManager)
        {
            this.tasks = tasks;
            this.categories = categories;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var all = await tasks.GetAllAsync();

            var users = userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    u.DisplayName,
                    u.FirstName,
                    u.LastName
                })
                .ToList()
                .ToDictionary(u => u.Id, u => u);

            var model = all.Select(t =>
            {
                users.TryGetValue(t.OwnerId, out var u);

                string ownerName =
                    !string.IsNullOrWhiteSpace(u?.DisplayName) ? u!.DisplayName! :
                    (!string.IsNullOrWhiteSpace(u?.FirstName) || !string.IsNullOrWhiteSpace(u?.LastName))
                        ? $"{u?.FirstName} {u?.LastName}".Trim()
                        : (u?.UserName ?? "Unknown");

                return new TaskListViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CategoryName = t.Category.Name,
                    Status = t.Status,
                    CreatedOn = t.CreatedOn,
                    OwnerEmail = u?.Email,
                    OwnerUserName = ownerName
                };
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> All(
            string? searchTerm,
            Guid? categoryId,
            int? status,
            int currentPage = 1)
        {
            var model = await tasks.GetAllAsync(
                UserId,
                searchTerm,
                categoryId,
                status,
                currentPage,
                5);

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
            {
                ModelState.AddModelError(nameof(vm.CategoryId), Categories.InvalidCategory);
            }

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

            try
            {
                await tasks.AddAsync(entity);
                TempData.SetSuccess(Tasks.Created);
                return RedirectToAction(nameof(All));
            }
            catch
            {
                TempData.SetError(Tasks.CreateFailed);
                vm.Categories = await categories.GetAllForSelectAsync();
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var task = IsAdmin
                ? await tasks.GetByIdAsync(id)
                : await tasks.GetByIdOwnedAsync(id, UserId);

            if (task == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Edit(Guid id)
        {
            var task = IsAdmin
                ? await tasks.GetByIdAsync(id)
                : await tasks.GetByIdOwnedAsync(id, UserId);

            if (task == null)
            {
                return NotFound();
            }

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
            var task = IsAdmin
                ? await tasks.GetByIdAsync(vm.Id)
                : await tasks.GetByIdOwnedAsync(vm.Id, UserId);

            if (task == null)
            {
                return NotFound();
            }

            if (!await categories.ExistsAsync(vm.CategoryId))
            {
                ModelState.AddModelError(nameof(vm.CategoryId), Categories.InvalidCategory);
            }

            if (!ModelState.IsValid)
            {
                vm.Categories = await categories.GetAllForSelectAsync();
                return View(vm);
            }

            task.Title = vm.Title;
            task.Description = vm.Description;
            task.CategoryId = vm.CategoryId;
            task.Status = vm.Status;

            try
            {
                await tasks.UpdateAsync(task);
                TempData.SetSuccess(Tasks.Updated);
                return RedirectToAction(nameof(All));
            }
            catch
            {
                TempData.SetError(Tasks.UpdateFailed);
                vm.Categories = await categories.GetAllForSelectAsync();
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = IsAdmin
                ? await tasks.GetByIdAsync(id)
                : await tasks.GetByIdOwnedAsync(id, UserId);

            if (task == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var task = IsAdmin
                ? await tasks.GetByIdAsync(id)
                : await tasks.GetByIdOwnedAsync(id, UserId);

            if (task == null)
            {
                return NotFound();
            }

            try
            {
                await tasks.DeleteAsync(task);
                TempData.SetSuccess(Tasks.Deleted);
            }
            catch
            {
                TempData.SetError(Tasks.DeleteFailed);
            }

            return RedirectToAction(nameof(All));
        }
    }
}