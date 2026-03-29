using LifeAdmin.Web.Infrastructure;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using ViewModels;
using ViewModels.TasksViewModels;
using static GCommon.NotificationMessages;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService tasks;
        private readonly ICategoryService categories;
        private readonly ITagService tagService;
        private readonly UserManager<ApplicationUser> userManager;

        private bool IsAdmin => User.IsInRole("Administrator");
        private string UserId => userManager.GetUserId(User)!;

        public TasksController(
            ITaskService tasks,
            ICategoryService categories,
            ITagService tagService,
            UserManager<ApplicationUser> userManager)
        {
            this.tasks = tasks;
            this.categories = categories;
            this.tagService = tagService;
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
                    OwnerUserName = ownerName,
                    Tags = t.TaskItemTags
                        .Select(tt => tt.Tag.Name)
                        .ToList()
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
                Status = WorkStatus.Planned,
                Tags = (await tagService.GetAllAsync())
                    .Select(t => new TagOptionViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFormViewModel vm)
        {
            if (!vm.CategoryId.HasValue || !await categories.ExistsAsync(vm.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(vm.CategoryId), Categories.InvalidCategory);
            }

            if (!ModelState.IsValid)
            {
                vm.Categories = await categories.GetAllForSelectAsync();
                vm.Tags = (await tagService.GetAllAsync())
                    .Select(t => new TagOptionViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();

                return View(vm);
            }

            var entity = new TaskItem
            {
                Title = vm.Title,
                Description = vm.Description,
                CategoryId = vm.CategoryId.Value,
                Status = vm.Status,
                OwnerId = UserId,
                CreatedOn = DateTime.UtcNow
            };

            foreach (var tagId in vm.SelectedTagIds)
            {
                entity.TaskItemTags.Add(new TaskItemTag
                {
                    TagId = tagId
                });
            }

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
                vm.Tags = (await tagService.GetAllAsync())
                    .Select(t => new TagOptionViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();

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
                CreatedOn = task.CreatedOn,
                Tags = task.TaskItemTags
                    .Select(tt => tt.Tag.Name)
                    .ToList()
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
                Categories = await categories.GetAllForSelectAsync(),
                Tags = (await tagService.GetAllAsync())
                    .Select(t => new TagOptionViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList(),
                SelectedTagIds = task.TaskItemTags
                    .Select(tt => tt.TagId)
                    .ToList()
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

            if (!vm.CategoryId.HasValue || !await categories.ExistsAsync(vm.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(vm.CategoryId), Categories.InvalidCategory);
            }

            if (!ModelState.IsValid)
            {
                vm.Categories = await categories.GetAllForSelectAsync();
                vm.Tags = (await tagService.GetAllAsync())
                    .Select(t => new TagOptionViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();

                return View(vm);
            }

            task.Title = vm.Title;
            task.Description = vm.Description;
            task.CategoryId = vm.CategoryId.Value;
            task.Status = vm.Status;

            task.TaskItemTags.Clear();

            foreach (var tagId in vm.SelectedTagIds)
            {
                task.TaskItemTags.Add(new TaskItemTag
                {
                    TaskItemId = task.Id,
                    TagId = tagId
                });
            }

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
                vm.Tags = (await tagService.GetAllAsync())
                    .Select(t => new TagOptionViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();

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

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Deleted()
        {
            var deletedTasks = await tasks.GetDeletedAsync();

            var model = deletedTasks.Select(t => new TaskListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                CategoryName = t.Category.Name,
                Status = t.Status,
                CreatedOn = t.CreatedOn,
                OwnerUserName = !string.IsNullOrWhiteSpace(t.Owner.DisplayName)
                    ? t.Owner.DisplayName
                    : t.Owner.UserName,
                OwnerEmail = t.Owner.Email,
                Tags = t.TaskItemTags.Select(tt => tt.Tag.Name).ToList()
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var task = await tasks.GetDeletedByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            await tasks.RestoreAsync(task);
            TempData.SetSuccess("Task restored successfully.");

            return RedirectToAction(nameof(Deleted));
        }
    }
}