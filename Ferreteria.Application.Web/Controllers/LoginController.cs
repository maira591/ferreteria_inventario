using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Utils;
using Ferreteria.DataAccess.Model.Generics;
using Ferreteria.Domain.LoginService;
using Ferreteria.Domain.ViewModel;
using Ferreteria.Domain.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace Ferreteria.Application.Website.Controllers
{
    public class LoginController : Controller
    {

        #region Private fields
        private readonly ILoginService _loginService;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;
        private readonly UserBasicModel _currentUser;
        #endregion

        #region Constructor
        public LoginController(IUserBasicModel userBasicModel, IHttpContextAccessor accessor, IMapper mapper, ILoginService loginService)
        {
            _loginService = loginService;
            _accessor = accessor;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _currentUser = _userBasicModel.GetCurrentUser();
        }
        #endregion

        #region Methods
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            if (_currentUser != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Index(LoginModel model)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            var response = new ResponseVM<UserModel>();

            if (!ModelState.IsValid)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Los datos ingresados son incorrectos. Verifique la información ingresada e intente nuevamente.";
                return Json(response);
            }

            try
            {
                var loginResponse = await _loginService.Login(new LoginViewModel { Password = model.Password, Username = model.Username });

                if (loginResponse.Status != HttpStatusCode.OK)
                {
                    return Json(loginResponse);
                }


                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, loginResponse.Body.Email)
                };

                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(loginResponse.Body));

                return Json(loginResponse);
            }
            catch (Exception e)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = "Ocurrió un error, intente más tarde o comuniquese con el administrador del sistema.";
                return Json(response);
            }
        }

        /// <summary>
        /// Logoff the user from the application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LogOff()
        {
            try
            {
                var response = _userBasicModel.GetCurrentUser();
                if (response == null)
                {
                    return RedirectToAction("Index");
                }
                if (HttpContext.Request.Cookies.Count > 0)
                {
                    var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
                    foreach (var cookie in siteCookies)
                    {
                        Response.Cookies.Delete(cookie.Key);
                    }
                }
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult AjaxLogOff()
        {
            try
            {
                var response = _userBasicModel.GetCurrentUser();
                return Json("{message:\"OK\"}");
            }
            catch (Exception)
            {
                return Json("{message:\"error\"}");
            }
            finally
            {
                HttpContext.Session.Clear();
            }
        }

        [HttpPost]
        public void AjaxReactiveSession()
        {
            Console.WriteLine(1);
        }

        [HttpGet]
        public IActionResult RememberPassword()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            return View();
        }

        [HttpGet]
        public IActionResult SuccessPasswordChange()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            var model = _accessor.HttpContext.Session.GetObject<PasswordChange>("DataPasswordChange");
            if (model == null)
            {
                return View("Index");
            }

            ViewBag.UrlBase = Common.GetUrlBase(Request);
            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordChange(PasswordChange model)
        {
            var passwordPolicies = _loginService.PasswordPolicies();
            model.PasswordPolicies = passwordPolicies.Body;

            if (!ModelState.IsValid)
                return View(model);

            //var recoverPasswordResult = RecoverPassword(model);

            //if (recoverPasswordResult.Status == HttpStatusCode.OK)
            //{
            //    _accessor.HttpContext.Session.Remove("DataPasswordChange");
            //    return View("SuccessPasswordChange");
            //}

            ModelState.AddModelError(string.Empty, $"No se pudo cambiar la contraseña: ");

            return View(model);
        }



        [HttpGet]
        public ActionResult VerifyToken(int UserId, int TimeToLive, string UserName)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            VerifyToken model = new()
            {
                UserId = UserId,
                TimeToLive = TimeToLive,
                UserName = UserName,
            };
            return View(model);
        }


        /// <summary>
        /// Establece el usuario en la sesion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetUser()
        {
            var response = new ResponseVM<UserModel>();

            try
            {
                var user = _mapper.Map<UserBasicModel>(JsonConvert.DeserializeObject<UserModelComplete>(HttpContext.Session.GetString("user")));
                _userBasicModel.SetCurrentUser(user);
                response.Message = "Ingresando...";
                response.Status = HttpStatusCode.OK;
                return Json(response);
            }
            catch (Exception e)
            {
                response.Message = $"Tiempo agotado para la petición: {e.Message}.\nVuelva a intentarlo.";
                response.Status = HttpStatusCode.InternalServerError;
                return Json(response);
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Cambiar la contraseña del usuario
        /// </summary>
        /// <returns></returns>        
        public ActionResult ChangePassword()
        {
            var model = new ChangePassword();
            var passwordPolicies = _loginService.PasswordPolicies();
            model.PasswordPolicies = passwordPolicies.Body;

            return View("ChangePassword", model);
        }

        /// <summary>
        /// Actualizar información del usuario
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult UpdateInfoUser()
        {
            var user = _userBasicModel.GetCurrentUser();
            ViewBag.ListTypeUsers = new List<SelectListItem>();

            return View("UpdateInfoUser", user);
        }

        [HttpPost]
        public JsonResult UpdateInfoUser(UserBasicModel model)
        {
            GenericResponse<string> response = new();

            /*
            _userService.UpdateInfoUser(
            new UpdateInfoUser()
            {
                Email = model.Email,
                Id = model.UserId,
                Name = model.Name,
            });*/

            if (response.Status == HttpStatusCode.OK)
            {
                var user = _userBasicModel.GetCurrentUser();
                user.Name = model.Name;
                user.Email = model.Email;

                _userBasicModel.SetCurrentUser(user);
                response.Message = "Información guardada correctamente.";
            }

            return Json(response);
        }

        [HttpPost]
        public JsonResult ChangePasswordResult(ChangePassword model)
        {
            var user = _userBasicModel.GetCurrentUser();
            GenericResponse<string> response;
            var request = new PasswordChangeResquest
            {
                UserName = user.Email,
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword
            };
            try
            {
                if (model.NewPassword == model.ConfirmNewPassword)
                {
                    response = _loginService.ChangePassword(request);
                }
                else
                {
                    response = GenericResponse<string>.GetErrorResponse(500, new Exception("La nueva contraseña y la confirmación de contraseña deben ser iguales."));
                }
            }
            catch (Exception ex)
            {
                response = GenericResponse<string>.GetErrorResponse(500, ex);
            }

            if (response.Message == "Usuario bloqueado")
            {
                HttpContext.Session.Clear();
            }

            return Json(response);
        }

        #endregion
    }
}
