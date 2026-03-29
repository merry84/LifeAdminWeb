using LifeAdminServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeAdmin.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> All()
        {
            var users = await userService.GetAllAsync();
            return View(users);
        }

     
        public async Task<IActionResult> Details(string id)
        {
            var model = await userService.GetDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleAdmin(string id)
        {
            await userService.ToggleAdminRoleAsync(id);

            return RedirectToAction(nameof(All));
        }
    }
}