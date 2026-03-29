using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LifeAdminModels.Models;
using GCommon;
using ViewModels.ProfilesViewModels;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileController(
            IProfileService profileService,
            UserManager<ApplicationUser> userManager)
        {
            this.profileService = profileService;
            this.userManager = userManager;
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