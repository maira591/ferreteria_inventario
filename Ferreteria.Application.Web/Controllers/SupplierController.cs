using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Providers;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Domain.SupplierService;
using Ferreteria.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminSupplier)]
    public class SupplierController(ISupplierService supplierService, IMapper mapper) : Controller
    {
        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = mapper.Map<List<SupplierVM>, List<SupplierModel>>((await supplierService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear Proveedor";
            return PartialView("_CreateOrEdit", new SupplierModel());
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Proveedor";

            var supplier = await supplierService.GetByIdAsync(id);
            SupplierModel model = mapper.Map<SupplierModel>(supplier);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(SupplierModel supplier)
        {
            try
            {
                await supplierService.AddOrUpdateAsync(mapper.Map<SupplierVM>(supplier));
                return Json(JsonResponseFactory.SuccessResponse("Proveedor guardado correctamente."));
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
                await supplierService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el Proveedor correctamente."));
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
        public async Task<JsonResult> Validations(SupplierModel supplier)
        {
            string message = await supplierService.Validations(mapper.Map<SupplierVM>(supplier));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion
    }
}
