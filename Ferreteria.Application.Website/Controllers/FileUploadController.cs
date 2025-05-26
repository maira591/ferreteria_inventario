using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CooperativeService;
using Core.Domain.FileUploadService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.FileUploadSIG)]
    public class FileUploadController : Controller
    {
        #region Private fields
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly IDashboardService _dashboardService;
        private readonly ICooperativeService _cooperativeService;
        private readonly UserBasicModel _currentUser;
        #endregion

        #region Constructor
        public FileUploadController(IFileUploadService fileUploadService, IMapper mapper, IUserBasicModel userBasicModel, IValueCatalogService valueCatalogService,
                                    IDashboardService dashboardService, ICooperativeService cooperativeService
                                )
        {
            _fileUploadService = fileUploadService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _valueCatalogService = valueCatalogService;
            _dashboardService = dashboardService;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cooperativeService = cooperativeService;
        }
        #endregion

        #region Public Methods
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "FileUpload/";
            ViewBag.EntityTypeList = new List<SelectListItem>();
            ViewBag.EntityCodeList = new List<SelectListItem>();
            ViewBag.FileSizeDefault = int.Parse(_valueCatalogService.GetByCode(ConstantsValueCatalogs.PesoLimiteCargaArchivoSIG).Name);

            if (!string.IsNullOrWhiteSpace(_currentUser.Organization))
            {
                var cooperative = await _cooperativeService.GetByCode(int.Parse(_currentUser.Organization));
                ViewBag.CooperativeName = _currentUser.Organization + " - " + cooperative.Name + " / ";
                ViewBag.ShowCooperativeFilters = false;
                ViewBag.EntityCode = int.Parse(_currentUser.Organization);
                ViewBag.EntityType = cooperative.CooperativeType;
            }
            else
            {
                ViewBag.CooperativeName = string.Empty;
                ViewBag.ShowCooperativeFilters = true;
                List<ValueCatalogVM> entityTypeList = await _dashboardService.GetEntityTypeList();
                ViewBag.EntityTypeList = Common.LoadList(entityTypeList, "Name", "Code");
                ViewBag.EntityType = 0;
            }

            return View();
        }

        [HttpGet]
        public async Task<List<SelectListItem>> GetEntityList(int entityType)
        {
            List<CooperativeSimpleVM> entities = await _dashboardService.GetEntitiesList(entityType, true);
            var entityList = Common.LoadList(entities, "Name", "Code");
            if (entityList == null)
            {
                return new List<SelectListItem>();
            }
            return entityList;
        }

        [HttpPost]
        public async Task<IActionResult> ValidateFile(UplodFileVM model)
        {
            return Json(_fileUploadService.ValidateFile(model));
        }



        [HttpPost]
        public async Task<IActionResult> UploadFile(UplodFileVM model)
        {
            return Json(await _fileUploadService.UploadFile(model, _userBasicModel.GetCurrentUser().UserName));
        }

        #endregion

        #region Private Methods

        #endregion


    }
}
