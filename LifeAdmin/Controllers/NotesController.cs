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
    public class NotesController : Controller
    {
        private readonly INotesService notes;
        private readonly UserManager<ApplicationUser> userManager;

        private bool IsAdmin => User.IsInRole("Administrator");
        private string UserId => userManager.GetUserId(User)!;

        public NotesController(
            INotesService notes,
            UserManager<ApplicationUser> userManager)
        {
            this.notes = notes;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> All(string? searchTerm, int currentPage = 1)
        {
            var model = await notes.GetAllAsync(UserId, searchTerm, currentPage, 5);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new NoteFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var entity = new Note
            {
                Title = vm.Title,
                Content = vm.Content,
                OwnerId = UserId,
                CreatedOn = DateTime.UtcNow
            };

            try
            {
                await notes.AddAsync(entity);
                TempData.SetSuccess(Notes.Created);
                return RedirectToAction(nameof(All));
            }
            catch
            {
                TempData.SetError(Notes.CreateFailed);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var note = IsAdmin
                ? await notes.GetByIdAsync(id)
                : await notes.GetByIdOwnedAsync(id, UserId);

            if (note == null)
            {
                return NotFound();
            }

            var vm = new NoteDetailsViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedOn = note.CreatedOn
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var note = IsAdmin
                ? await notes.GetByIdAsync(id)
                : await notes.GetByIdOwnedAsync(id, UserId);

            if (note == null)
            {
                return NotFound();
            }

            var vm = new NoteFormViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NoteFormViewModel vm)
        {
            var note = IsAdmin
                ? await notes.GetByIdAsync(vm.Id)
                : await notes.GetByIdOwnedAsync(vm.Id, UserId);

            if (note == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            note.Title = vm.Title;
            note.Content = vm.Content;

            try
            {
                await notes.UpdateAsync(note);
                TempData.SetSuccess(Notes.Updated);
                return RedirectToAction(nameof(All));
            }
            catch
            {
                TempData.SetError(Notes.UpdateFailed);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var note = IsAdmin
                ? await notes.GetByIdAsync(id)
                : await notes.GetByIdOwnedAsync(id, UserId);

            if (note == null)
            {
                return NotFound();
            }

            var vm = new NoteDeleteViewModel
            {
                Id = note.Id,
                Title = note.Title
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var note = IsAdmin
                ? await notes.GetByIdAsync(id)
                : await notes.GetByIdOwnedAsync(id, UserId);

            if (note == null)
            {
                return NotFound();
            }

            try
            {
                await notes.DeleteAsync(note);
                TempData.SetSuccess(Notes.Deleted);
            }
            catch
            {
                TempData.SetError(Notes.DeleteFailed);
            }

            return RedirectToAction(nameof(All));
        }
    }
}