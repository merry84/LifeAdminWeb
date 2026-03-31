using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LifeAdminModels.Models;

using ViewModels.ProfilesViewModels;
using Microsoft.AspNetCore.Hosting;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment env;

        public ProfileController(
            IProfileService profileService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            this.profileService = profileService;
            this.userManager = userManager;
            this.env = env;
        }

        public async Task<IActionResult> Mine()
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var model = await profileService.GetProfileAsync(userId);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var model = await profileService.GetProfileForEditAsync(userId);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                var extension = Path.GetExtension(model.ProfileImageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.ProfileImageFile), "Only JPG, JPEG, PNG and WEBP files are allowed.");
                    return View(model);
                }

                if (model.ProfileImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(model.ProfileImageFile), "File size must be up to 2 MB.");
                    return View(model);
                }

                var uploadsFolder = Path.Combine(env.WebRootPath, "images", "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                model.CurrentProfileImageUrl = $"/images/profiles/{fileName}";
            }

            var success = await profileService.UpdateProfileAsync(userId, model);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Mine));
        }
    
    }
}