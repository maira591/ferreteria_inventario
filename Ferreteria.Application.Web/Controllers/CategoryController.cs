using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Domain.CategoryService;
using Ferreteria.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Website.Controllers
{
    public class CategoryController(ICategoryService categoryService, IMapper mapper) : Controller
    {
        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = mapper.Map<List<CategoryVM>, List<CategoryModel>>((await categoryService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear Categoría";
            return PartialView("_CreateOrEdit", new CategoryModel());
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Categoría";

            var supplier = await categoryService.GetByIdAsync(id);
            CategoryModel model = mapper.Map<CategoryModel>(supplier);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(CategoryModel supplier)
        {
            try
            {
                await categoryService.AddOrUpdateAsync(mapper.Map<CategoryVM>(supplier));
                return Json(JsonResponseFactory.SuccessResponse("Categoría guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await categoryService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el Categoría correctamente."));
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("||") >= 0)
                {
                    return Json(JsonResponseFactory.ErrorResponse(ex.Message.Replace("||", "")));
                }
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(CategoryModel supplier)
        {
            string message = await categoryService.Validations(mapper.Map<CategoryVM>(supplier));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion
    }
}
