using Ferreteria.Application.Website.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Application.Website.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            return View();
        }
    }
}
