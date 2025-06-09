using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Providers;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Domain.PermissionService;
using Ferreteria.Domain.ViewModel.Security;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminUser)]
    public class PermissionController(IPermissionService permissionService, IMapper mapper) : Controller
    {
        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = mapper.Map<List<PermissionVM>, List<PermissionModel>>((await permissionService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear Permiso";
            return PartialView("_CreateOrEdit", new PermissionModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Permiso";

            var permission = await permissionService.GetByIdAsync(id);
            PermissionModel model = mapper.Map<PermissionModel>(permission);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(PermissionModel permission)
        {
            try
            {
                await permissionService.AddOrUpdateAsync(mapper.Map<PermissionVM>(permission));
                return Json(JsonResponseFactory.SuccessResponse("Permiso guardado correctamente."));
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
                await permissionService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el Permiso correctamente."));
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
        public async Task<JsonResult> Validations(PermissionModel permission)
        {
            string message = await permissionService.Validations(mapper.Map<PermissionVM>(permission));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion
    }
}
