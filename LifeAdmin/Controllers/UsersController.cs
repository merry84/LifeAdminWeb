using LifeAdmin.Web.Infrastructure;
using LifeAdminModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace LifeAdmin.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
            => this.userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var vm = new ProfileViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                
                var current = await userManager.GetUserAsync(User);
                vm.Email = current?.Email ?? vm.Email;
                return View(vm);
            }

            var user = await userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var firstName = vm.FirstName.Trim();
            var lastName = vm.LastName.Trim();

            user.FirstName = firstName;
            user.LastName = lastName;

            user.DisplayName = string.IsNullOrWhiteSpace(vm.DisplayName)
                ? $"{firstName} {lastName}"
                : vm.DisplayName.Trim();

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }

                vm.Email = user.Email ?? vm.Email;
                return View(vm);
            }

            TempData.SetSuccess("Profile updated successfully!");
            return RedirectToAction(nameof(Profile));
        }
    }
}
