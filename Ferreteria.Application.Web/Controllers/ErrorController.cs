using Ferreteria.Application.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Application.Website.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("/Error/HandleError/{code:int}")]
        public IActionResult HandleError(int code)
        {
            return View(new HandleErrorModel() { StatusCode = code });
        }
    }
}
