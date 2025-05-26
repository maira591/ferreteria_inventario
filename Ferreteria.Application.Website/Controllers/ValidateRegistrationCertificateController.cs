using Core.Application.Website.Models;
using Core.Application.Website.Utils;
using Core.Domain.CaptchaService;
using Core.Domain.CatalogService;
using Core.Domain.CertificateRequestService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    public class ValidateRegistrationCertificateController : Controller
    {

        #region Private fields
        private readonly ICatalogService _catalogService;
        private readonly ICaptchaService _captchaService;
        private readonly ICertificateRequestService _certificateRequestService;
        #endregion

        #region Constructor
        public ValidateRegistrationCertificateController(ICatalogService catalogService, ICaptchaService captchaService, ICertificateRequestService certificateRequestService)
        {
            _captchaService = captchaService;
            _catalogService = catalogService;
            _certificateRequestService = certificateRequestService;
        }
        #endregion

        #region Methods
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string returnUrl)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "ValidateRegistrationCertificate/";

            ViewBag.ReturnUrl = returnUrl;

            var captchaCatalog = await _catalogService.GetByCodeAsync("Captcha");
            ViewBag.CaptchaKey = captchaCatalog.ValueCatalogs.FirstOrDefault(v => v.Code.Equals("RecaptchaPublicKey"))
                ?.Name;
            var valueCatalogCaptcha = captchaCatalog.ValueCatalogs
                .FirstOrDefault(v => v.Code.Equals("IsCaptchaEnabled"))?.Name;
            ViewBag.IsEnableCaptcha = Convert.ToBoolean(valueCatalogCaptcha);


            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(CertificateInscriptionModel model, string returnUrl)
        {
            var captchaCatalog = await _catalogService.GetByCodeAsync("Captcha");
            ViewBag.CaptchaKey = captchaCatalog.ValueCatalogs.FirstOrDefault(v => v.Code.Equals("RecaptchaPublicKey"))?.Name;
            var isCaptchaEnable = Convert.ToBoolean(captchaCatalog.ValueCatalogs
                .FirstOrDefault(v => v.Code.Equals("IsCaptchaEnabled"))?.Name);
            ViewBag.IsEnableCaptcha = isCaptchaEnable;
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "ValidateRegistrationCertificate/";

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Datos incorrectos."
                });
            }

            try
            {
#if !DEBUG
                if (isCaptchaEnable)
                {
                    var validateResult = await _captchaService.Validate(Request.Form["g-recaptcha-response"]);
                    if (!validateResult.Success)
                    {

                     return Json(new {
                        Status = false,
                        Message = "Se debe validar el captcha para continuar."
                    });
                    }
                }
                
#endif
                Guid codeCertificate = Guid.Empty;
                if (!Guid.TryParse(model.CertificateCode, out codeCertificate))
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "El código ingresado no es valido."
                    });
                }

                var result = await _certificateRequestService.VerifyAndGenerateCertificate(codeCertificate);

                if (result.Exists)
                {
                    return Json(new
                    {
                        result.ValidationCode,
                        Status = result.Exists,
                        BytesFile = File(result.BytesFile, "application/pdf"),
                    });
                }
                else
                {
                    return Json(new
                    {
                        result.ValidationCode,
                        Status = result.Exists,
                        Message = "El código de verficación no fue encontrado."
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    Status = false,
                    Message = "Ocurrió un error, intente más tarde o comuniquese con el administrador del sistema."
                });

            }
        }

        #endregion

    }
}
