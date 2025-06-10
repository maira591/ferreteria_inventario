using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Providers;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Domain.PermissionService;
using Ferreteria.Domain.RoleService;
using Ferreteria.Domain.UserService;
using Ferreteria.Domain.ViewModel.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminUser)]
    public class UserController(IUserService userService, IMapper mapper, IRoleService roleService) : Controller
    {
        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = mapper.Map<List<UserVM>, List<UserInfoModel>>((await userService.GetAllAsync()).ToList());
            model.ForEach(e => e.RolesNames = (e.UserRoles.Count > 0) ?
                                string.Join(", ", e.UserRoles.Select(x => x.Role.Name)) : "");

            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Usuario";
            ViewBag.ListRoles = await RoleList();

            return PartialView("_CreateOrEdit", new UserInfoModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Usuario";
            ViewBag.ListRoles = await RoleList();
            var user = await userService.GetByIdAsync(id);
            UserInfoModel model = mapper.Map<UserInfoModel>(user);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(UserInfoModel user)
        {
            try
            {

                await userService.AddOrUpdateAsync(mapper.Map<UserVM>(user));

                return Json(JsonResponseFactory.SuccessResponse("Usuario guardado correctamente."));
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
                await userService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el Usuario correctamente."));
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
        public async Task<JsonResult> Validations(UserInfoModel user)
        {
            string message = await userService.Validations(mapper.Map<UserVM>(user));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> RoleList()
        {
            List<RoleVM> users = (await roleService.GetAllAsync()).Where(x => x.IsEnabled).ToList();
            var list = Common.LoadList(users, "Name", "RoleId");

            if (list == null)
            {
                return [];
            }

            return list;
        }
        #endregion
    }
}
