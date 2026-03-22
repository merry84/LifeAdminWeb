using Microsoft.AspNetCore.Mvc;

namespace LifeAdmin.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("~/Views/Shared/Error404.cshtml");
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            return View("~/Views/Shared/Error500.cshtml");
        }
    }
}