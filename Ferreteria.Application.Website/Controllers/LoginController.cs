using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Utils;
using Core.DataAccess.Model.Generics;
using Core.Domain.CaptchaService;
using Core.Domain.CatalogService;
using Core.Domain.CooperativeService;
using Core.Domain.EnvioNotificacionesService;
using Core.Domain.LoginService;
using Core.Domain.Recaudo.CargoService;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.TokenService;
using Core.Domain.UserService;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Core.Domain.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    public class LoginController : Controller
    {

        #region Private fields
        private readonly ICatalogService _catalogService;
        private readonly ICooperativeService _cooperativeService;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly ILoginService _loginService;
        private readonly ICaptchaService _captchaService;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IHttpContextAccessor _accessor;
        private readonly INotificationCatalog _notificationCatalog;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly UserBasicModel _currentUser;
        private readonly ICargoService _cargoService;
        #endregion

        #region Constructor
        public LoginController(ICatalogService catalogService, ICaptchaService captchaService, IValueCatalogService valueCatalogService,
            IUserBasicModel userBasicModel, IHttpContextAccessor accessor, IMapper mapper, ILoginService loginService,
            ITokenService tokenService, ICooperativeService cooperativeService, INotificationCatalog notificationCatalog, IUserService userService,
            ICargoService cargoService )
        {
            _userService = userService;
            _notificationCatalog = notificationCatalog;
            _cooperativeService = cooperativeService;
            _tokenService = tokenService;
            _captchaService = captchaService;
            _valueCatalogService = valueCatalogService;
            _loginService = loginService;
            _accessor = accessor;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cargoService = cargoService;
        }
        #endregion

        #region Methods
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string returnUrl)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            if (_currentUser != null)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;

            var captchaCatalog = await _catalogService.GetByCodeAsync("Captcha");
            ViewBag.CaptchaKey = captchaCatalog.ValueCatalogs.FirstOrDefault(v => v.Code.Equals("RecaptchaPublicKey"))
                ?.Name;
            var valueCatalogCaptcha = captchaCatalog.ValueCatalogs
                .FirstOrDefault(v => v.Code.Equals("IsCaptchaEnabled"))?.Name;
            ViewBag.IsEnableCaptcha = Convert.ToBoolean(valueCatalogCaptcha);

            var rutasCatalog = await _catalogService.GetByCodeAsync("RutasEnlaces");
            ViewBag.TerminosUrl = rutasCatalog.ValueCatalogs.FirstOrDefault(v => v.Code.Equals("TerminosUrl"))?.Name;
            ViewBag.HabeasDataUrl = rutasCatalog.ValueCatalogs.FirstOrDefault(v => v.Code.Equals("HabeasUrl"))?.Name;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Index(LoginModel model, string returnUrl)
        {
            var captchaCatalog = await _catalogService.GetByCodeAsync("Captcha");
            ViewBag.CaptchaKey = captchaCatalog.ValueCatalogs.FirstOrDefault(v => v.Code.Equals("RecaptchaPublicKey"))?.Name;
            var isCaptchaEnable = Convert.ToBoolean(captchaCatalog.ValueCatalogs
                .FirstOrDefault(v => v.Code.Equals("IsCaptchaEnabled"))?.Name);
            ViewBag.IsEnableCaptcha = isCaptchaEnable;
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
#if !DEBUG
                if (isCaptchaEnable)
                {
                    var validateResult = await _captchaService.Validate(Request.Form["g-recaptcha-response"]);
                    if (!validateResult.Success)
                    {
                        response.Status = HttpStatusCode.PreconditionFailed;
                        response.Message = "Se debe validar el captcha para continuar.";
                        return Json(response);
                    }
                }
#endif

                var loginResponse = _loginService.LogIn(new LoginViewModel
                { Password = model.Password, Username = model.Username });
                response = _mapper.Map<ResponseVM<UserModel>>(loginResponse);

                if (loginResponse.Status != HttpStatusCode.OK)
                {
                    return Json(response);
                }

                HttpContext.Session.SetString("TerminosUrl", (await _valueCatalogService.GetByCodeAsync("TerminosUrl")).Name);
                HttpContext.Session.SetString("HabeasUrl", (await _valueCatalogService.GetByCodeAsync("HabeasUrl")).Name);
                HttpContext.Session.SetString("helpUrl", (await _valueCatalogService.GetByCodeAsync("AyudaURL")).Name);
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(loginResponse.Body));

                response.Body = new UserModel
                {
                    AcceptedHabeasData = loginResponse.Body.AcceptedHabeasData,
                    AcceptedTermsAndConditions = loginResponse.Body.AcceptedTermsAndConditions
                };

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, loginResponse.Body.UserName)
                };
                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                HttpContext.Session.SetInt32("Timeout", int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeoutSessionInMinutes"]));
                HttpContext.Session.SetInt32("timeoutSessionInMilliseconds", int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeoutSessionInMilliseconds"]));
                HttpContext.Session.SetInt32("timeWaitingForAnswerInactivitySeconds", int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeWaitingForAnswerInactivitySeconds"]));

                return Json(response);
            }
            catch
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
                _loginService.LogOut(new TokenModel { UserToken = response.Token });
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
                if (response != null)
                    _loginService.LogOut(new TokenModel { UserToken = response.Token });
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

            var recoverPasswordResult = RecoverPassword(model);

            if (recoverPasswordResult.Status == HttpStatusCode.OK)
            {
                _accessor.HttpContext.Session.Remove("DataPasswordChange");
                return View("SuccessPasswordChange");
            }

            ModelState.AddModelError(string.Empty, $"No se pudo cambiar la contraseña: {recoverPasswordResult.Message}");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RememberPassword(RememberPassword model)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            var responseView = new ResponseVM<RememberPassword>();
            try
            {

                if (!ModelState.IsValid)
                {
                    var errorMessagge = ModelState.Values.First().Errors.First().ErrorMessage;
                    responseView.Status = HttpStatusCode.InternalServerError;
                    responseView.Message = errorMessagge;
                    return Json(responseView);
                }

                var response = _userService.Get(model.UserName);

                if (response.Body == null)
                {
                    responseView.Status = HttpStatusCode.InternalServerError;
                    responseView.Message = "El usuario no fue encontrado en la base de datos. intente nuevamente.";
                    return Json(responseView);
                }

                //Genera los elementos del cuerpo del mensaje
                var emailConfig = _catalogService.GetByCode("Email");
                if (emailConfig == null)
                    throw new Exception("No se logró obtener la configuración del correo.");

                var imagenHeader =
                    $"{Common.GetUrlBase(Request)}{emailConfig.ValueCatalogs.FirstOrDefault(e => e.Code.Equals("ImagenHeader"))?.Name}";
                var emailBody = emailConfig.ValueCatalogs.FirstOrDefault(e => e.Code.Equals("CuerpoVerificacionToken"))?.Name;
                var imagenFooter =
                    $"{Common.GetUrlBase(Request)}{emailConfig.ValueCatalogs.FirstOrDefault(e => e.Code.Equals("ImagenFooter"))?.Name}";

                var tokenTimeToLive = GetTokenTimeToLive();

                var user = response.Body;

                var token = GenerateToken(model.UserName);

                var idCooperative = Guid.Empty.ToString();

                if (!string.IsNullOrEmpty(user.Organization))
                {
                    var cooperative = await _cooperativeService.GetByCode(int.Parse(user.Organization));
                    if (cooperative != null)
                    {
                        idCooperative = cooperative.Code.ToString();
                    }
                }
                emailBody = emailBody.Replace("{NombreAplicacion}", _valueCatalogService.GetApplicationInfo().Result.Name);

                var kvpList = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ImagenHeader", imagenHeader),
                    new KeyValuePair<string, string>("CuerpoDelCorreo", emailBody),
                    new KeyValuePair<string, string>("Name", user.Name),
                    new KeyValuePair<string, string>("UserName", user.UserName),
                    new KeyValuePair<string, string>("Token", token),
                    new KeyValuePair<string, string>("TimeToLive", (tokenTimeToLive / 60).ToString()),
                    new KeyValuePair<string, string>("ImagenFooter", imagenFooter)
                };

                await _notificationCatalog.SendNotification("RecordarContrasena", kvpList, idCooperative, user.Email);

                var verificationModel = new VerifyToken
                {
                    UserId = user.Id,
                    UserName = model.UserName,
                    TimeToLive = tokenTimeToLive,
                    Token = ""
                };

                return Json(verificationModel);
            }
            catch
            {
                HttpContext.Response.StatusCode = 500;
                responseView.Status = HttpStatusCode.InternalServerError;
                responseView.Message = "Ocurrió un error inesperado.";
                return Json(responseView);
            }
        }

        [HttpGet]
        public ActionResult VerifyToken(Guid UserId, int TimeToLive, string UserName)
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

        [HttpPost]
        public ActionResult VerifyTokenn(VerifyToken model)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            var userName = model.UserName;
            var responseView = new ResponseVM<RememberPassword>();

            var isValidToken = _tokenService.ValidateToken(userName, userName, model.Token, model.TimeToLive);
            if (!isValidToken)
            {
                responseView.Status = HttpStatusCode.InternalServerError;
                responseView.Message = "El Token ingresado no es válido.";
                return Json(responseView);
            }

            var passwordPolicies = _loginService.PasswordPolicies();
            var passwordChangeModel = new PasswordChange
            {
                UserId = model.UserId,
                UserName = model.UserName,
                PasswordPolicies = passwordPolicies.Body
            };

            _accessor.HttpContext.Session.SetObject("DataPasswordChange", passwordChangeModel);
            return View("PasswordChange", passwordChangeModel);
        }

        [HttpPost]
        public JsonResult AgreeTerms()
        {
            var response = new ResponseVM<UserModel>();
            try
            {
                var user = JsonConvert.DeserializeObject<UserModelComplete>(HttpContext.Session.GetString("user"));
                user.AcceptedHabeasData = true;
                user.AcceptedTermsAndConditions = true;
                response = _userService.CreateOrUpdate(user);
                response.Status = HttpStatusCode.OK;
                return Json(response);
            }
            catch (Exception e)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Message = $"Tiempo agotado para la petición: {e.Message}.\nVuelva a intentarlo.";
                return Json(response);
            }
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
        private string GenerateToken(string userName)
        {
            var tokenLiveTime = GetTokenTimeToLive();
            var currentToken = _tokenService.GenerateToken(userName, userName, tokenLiveTime);
            return currentToken;
        }

        private int GetTokenTimeToLive()
        {
            var liveTime = _valueCatalogService.GetValueCatalogName("tokenLiveTime");
            return int.Parse(liveTime);
        }
        private ResponseVM<RememberPassword> RecoverPassword(PasswordChange model)
        {
            var responseView = new ResponseVM<RememberPassword>();
            var objResponse = _loginService.RecoverPassword(new RecoverPasswordResquest
            {
                UserName = model.UserName,
                Password = model.NewPassword
            });

            responseView.Status = objResponse.Status;
            if (objResponse.Status != HttpStatusCode.OK)
            {

                responseView.Message = objResponse.Message;
                return responseView;
            }

            return responseView;
        }

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
            
            if (user.Organization != null)
            {
                var nameCooperative = _cooperativeService.GetByCode(int.Parse(user.Organization)).Result.Name.ToString();

                user.OrganizationName = $"{user.Organization} - {nameCooperative}";
            }

            var cargo = _cargoService.GetByEmail(user.Email).Result;

            ViewBag.ListTypeUsers = GetListPosition(user.Organization != null);

            if (cargo != null)
            {
                user.Position = cargo.CodigoCargo;
            }

            return View("UpdateInfoUser", user);
        }

        [HttpPost]
        public JsonResult UpdateInfoUser(UserBasicModel model)
        {
            GenericResponse<string> response =  _userService.UpdateInfoUser(
                new UpdateInfoUser()
                {
                    Email = model.Email,
                    Id = model.Id,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber
                });

            if (response.Status == HttpStatusCode.OK)
            {
                var user = _userBasicModel.GetCurrentUser();
                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                _userBasicModel.SetCurrentUser(user);
                _cargoService.UpdateInfoPosition(model.Email, model.Position);
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
                UserName = user.UserName,
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
                _loginService.LogOut(new TokenModel { UserToken = user.Token });
                HttpContext.Session.Clear();
            }

            return Json(response);
        }

        #endregion


        #region Private Methods
        private List<SelectListItem> GetListPosition(bool isCooperative)
        {
            if (isCooperative)
            {
                 return Common.LoadList(_cargoService.GetByUserType(TyeUser.Cooperative).Result, "Descripcion", "CodigoCargo");
            }
            else
            {
                return Common.LoadList(_cargoService.GetByUserType(TyeUser.Fogacoop).Result, "Descripcion", "CodigoCargo");
            }
        }


        #endregion
    }
}
