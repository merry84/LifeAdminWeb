using LifeAdmin.Web.Infrastructure;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using static GCommon.DataConstants.Document;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService documents;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        private bool IsAdmin => User.IsInRole("Administrator");
        private string UserId => userManager.GetUserId(User)!;

        public DocumentsController(
            IDocumentService documents,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            this.documents = documents;
            this.userManager = userManager;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (IsAdmin)
            {
                var allDocs = await documents.GetAllAsync();
                return View(allDocs);
            }

            var myDocs = await documents.GetMineAsync(UserId);
            return View(myDocs);
        }

        [HttpGet]
        public IActionResult Upload()
            => View(new DocumentUploadViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(DocumentUploadViewModel vm)
        {
            if (vm.File == null || vm.File.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.File), "Please select a file.");
            }

            if (vm.File != null && vm.File.Length > MaxFileSize)
            {
                ModelState.AddModelError(nameof(vm.File), "File size must be up to 5 MB.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string uploadsFolder = Path.Combine(environment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsFolder);

            string extension = Path.GetExtension(vm.File.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, storedFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await vm.File.CopyToAsync(stream);
            }

            var document = new Document
            {
                Title = vm.Title,
                FileName = vm.File.FileName,
                StoredFileName = storedFileName,
                ContentType = vm.File.ContentType ?? "application/octet-stream",
                FileSize = vm.File.Length,
                OwnerId = UserId,
                UploadedOn = DateTime.UtcNow
            };

            await documents.AddAsync(document);
            TempData.SetSuccess("Document uploaded successfully.");

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            var document = IsAdmin
                ? await documents.GetByIdAsync(id)
                : await documents.GetByIdOwnedAsync(id, UserId);

            if (document == null)
            {
                return NotFound();
            }

            string filePath = Path.Combine(environment.WebRootPath, "uploads", "documents", document.StoredFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            byte[] bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, document.ContentType, document.FileName);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var document = IsAdmin
                ? await documents.GetByIdAsync(id)
                : await documents.GetByIdOwnedAsync(id, UserId);

            if (document == null)
            {
                return NotFound();
            }

            var vm = new DocumentDeleteViewModel
            {
                Id = document.Id,
                Title = document.Title,
                FileName = document.FileName
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var document = IsAdmin
                ? await documents.GetByIdAsync(id)
                : await documents.GetByIdOwnedAsync(id, UserId);

            if (document == null)
            {
                return NotFound();
            }

            string filePath = Path.Combine(environment.WebRootPath, "uploads", "documents", document.StoredFileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await documents.DeleteAsync(document);
            TempData.SetSuccess("Document deleted successfully.");

            return RedirectToAction(nameof(All));
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Deleted()
        {
            var model = await documents.GetDeletedAsync();
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var document = await documents.GetDeletedByIdAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            await documents.RestoreAsync(document);
            TempData.SetSuccess("Document restored successfully.");

            return RedirectToAction(nameof(Deleted));
        }
    }
}