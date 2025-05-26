using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Website.Controllers
{
    public class ResultsController : Controller
    {
        // GET: ResultadosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ResultadosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ResultadosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ResultadosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ResultadosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ResultadosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ResultadosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ResultadosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
