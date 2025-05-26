using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CertificateRequestService;
using Core.Domain.CooperativeService;
using Core.Domain.OfficeService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.GenerateCertificateCooperativeAdmin)]
    public class GenerateCertificateCoopAdminController : Controller
    {
        #region Private Fields
        private readonly UserBasicModel _currentUser;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IMapper _mapper;
        private readonly IOfficeService _officeService;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly ICooperativeService _cooperativeService;
        private readonly ICertificateRequestService _certificateRequestService;
        private readonly IDashboardService _dashboardService;

        #endregion

        #region Constructor
        public GenerateCertificateCoopAdminController(IUserBasicModel userBasicModel, ICertificateRequestService certificateRequestService,
                                   IMapper mapper, IOfficeService officeService, IValueCatalogService valueCatalogService, ICooperativeService cooperativeService, IDashboardService dashboardService)
        {
            _certificateRequestService = certificateRequestService;
            _userBasicModel = userBasicModel;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cooperativeService = cooperativeService;
            _mapper = mapper;
            _officeService = officeService;
            _valueCatalogService = valueCatalogService;
            _dashboardService = dashboardService;
        }
        #endregion

        #region Methods
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.EntityTypeList = new List<SelectListItem>();
            ViewBag.EntityCodeList = new List<SelectListItem>();

            if (string.IsNullOrWhiteSpace(_currentUser.Organization))
            {
                ViewBag.CooperativeName = string.Empty;
                ViewBag.ShowCooperativeFilters = true;
                List<ValueCatalogVM> entityTypeList = await _dashboardService.GetEntityTypeList();
                ViewBag.EntityTypeList = Common.LoadList(entityTypeList, "Name", "Code");
                ViewBag.EntityType = 0;
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

        public async Task<List<SelectListItem>> GetEntityList(int entityType)
        {
            List<CooperativeSimpleVM> entities = await _dashboardService.GetEntitiesList(entityType);
            var entityList = Common.LoadList(entities, "Name", "Code");
            if (entityList == null)
            {
                return new List<SelectListItem>();
            }
            return entityList;
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
