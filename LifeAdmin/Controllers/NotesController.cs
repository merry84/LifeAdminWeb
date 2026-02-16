using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly INotesService notesService;
        private readonly UserManager<ApplicationUser> userManager;

        public NotesController(INotesService notesService, UserManager<ApplicationUser> userManager)
        {
            this.notesService = notesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> All()
        {
            var userId = userManager.GetUserId(User)!;
            var model = await notesService.GetMineAsync(userId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create() => View(new NoteFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = userManager.GetUserId(User)!;
            await notesService.CreateAsync(model, userId);
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userManager.GetUserId(User)!;
            var model = await notesService.GetForEditAsync(id, userId);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NoteFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = userManager.GetUserId(User)!;

            if (!await notesService.ExistsOwnedAsync(model.Id, userId))
                return NotFound();

            await notesService.EditAsync(model, userId);
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userManager.GetUserId(User)!;
            await notesService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(All));
        }
    }
}
