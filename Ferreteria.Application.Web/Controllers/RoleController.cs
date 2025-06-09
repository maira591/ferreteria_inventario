using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Providers;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Domain.LoadTypeService;
using Ferreteria.Domain.PermissionService;
using Ferreteria.Domain.RoleService;
using Ferreteria.Domain.ViewModel.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminUser)]
    public class RoleController(IRoleService roleService, IMapper mapper, IPermissionService permissionService) : Controller
    {
        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = mapper.Map<List<RoleVM>, List<RoleModel>>((await roleService.GetAllAsync()).ToList());
            model.ForEach(e => e.PermissionsNames = (e.RolePermissions.Count > 0) ?
                                string.Join(", ", e.RolePermissions.Select(x => x.Permission.Name)) : "");

            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Rol";
            ViewBag.ListPermissions = await PermissionList();

            return PartialView("_CreateOrEdit", new RoleModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Rol";
            ViewBag.ListPermissions = await PermissionList();
            var permission = await roleService.GetByIdAsync(id);
            RoleModel model = mapper.Map<RoleModel>(permission);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(RoleModel permission)
        {
            try
            {
                
                await roleService.AddOrUpdateAsync(mapper.Map<RoleVM>(permission));

                return Json(JsonResponseFactory.SuccessResponse("Rol guardado correctamente."));
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
                await roleService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el Rol correctamente."));
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
        public async Task<JsonResult> Validations(RoleModel permission)
        {
            string message = await roleService.Validations(mapper.Map<RoleVM>(permission));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> PermissionList()
        {
            List<PermissionVM> permissions = (await permissionService.GetAllAsync()).Where(x => x.IsEnabled).ToList();
            var list = Common.LoadList(permissions, "Name", "PermissionId");

            if (list == null)
            {
                return [];
            }

            return list;
        }
        #endregion
    }
}
