using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CertificateRequestService;
using Core.Domain.CooperativeService;
using Core.Domain.OfficeService;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.GenerateCertificateCooperative)]
    public class GenerateCertificateCooperativeController : Controller
    {
        #region Private Fields
        private readonly UserBasicModel _currentUser;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IMapper _mapper;
        private readonly IOfficeService _officeService;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly ICooperativeService _cooperativeService;
        private readonly ICertificateRequestService _certificateRequestService;

        #endregion

        #region Constructor
        public GenerateCertificateCooperativeController(IUserBasicModel userBasicModel, ICertificateRequestService certificateRequestService,
                                   IMapper mapper, IOfficeService officeService, IValueCatalogService valueCatalogService, ICooperativeService cooperativeService)
        {
            _certificateRequestService = certificateRequestService;
            _userBasicModel = userBasicModel;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cooperativeService = cooperativeService;
            _mapper = mapper;
            _officeService = officeService;
            _valueCatalogService = valueCatalogService;
        }
        #endregion

        #region Methods
        [Auth(PrivilegesEnum.GenerateCertificateCooperative)]
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            if (!string.IsNullOrWhiteSpace(_currentUser.Organization))
            {
                var cooperative = await _cooperativeService.GetByCode(int.Parse(_currentUser.Organization));
                ViewBag.CooperativeName = _currentUser.Organization + " - " + cooperative.Name;
                ViewBag.ShowCooperativeFilters = false;
                ViewBag.EntityCode = int.Parse(_currentUser.Organization);
                ViewBag.EntityType = cooperative.CooperativeType;
            }
            else
            {
                return new RedirectResult("~/Home/AccessDenied");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(CertificateRequestModel certificateRequest)
        {
            try
            {

                CertificateRequestVM certificateRequestVM = new() { EntityCode = certificateRequest.EntityCode, EntityTypeId = certificateRequest.EntityType, CreatedBy = _currentUser.UserName };
                certificateRequestVM.ValidationCode = Guid.NewGuid();
                await _certificateRequestService.AddOrUpdateAsync(certificateRequestVM);
                return Json(new { Status = true, ValidateCode = certificateRequestVM.ValidationCode, certificateRequestVM.Consecutive });
            }
            catch
            {
                return Json(new { Status = false, Message = "Ocurrió un error inesperado al generar." });
            }
        }

        [HttpPost]
        public async Task<FileResult> GenerateCertificateFile(Guid validateCode)
        {
            byte[] pdfFile = await _certificateRequestService.GenerateCertificatePDF(validateCode);
            return File(pdfFile, "application/pdf");
        }

        #endregion
    }
}
