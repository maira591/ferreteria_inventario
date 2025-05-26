using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CooperativeService;
using Core.Domain.FileDownloadService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.FileDownloadSIG)]
    public class FileDownloadController : Controller
    {
        #region Private fields
        private readonly IFileDownloadService _fileDownloadService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IValueCatalogService _valueCatatalogService;
        private readonly IDashboardService _dashboardService;
        private readonly ICooperativeService _cooperativeService;
        private readonly UserBasicModel _currentUser;
        #endregion

        #region Constructor
        public FileDownloadController(IFileDownloadService fileDownloadService, IMapper mapper, IUserBasicModel userBasicModel, IValueCatalogService valueCatatalogService,
                                    IDashboardService dashboardService, ICooperativeService cooperativeService
                                )
        {
            _fileDownloadService = fileDownloadService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _valueCatatalogService = valueCatatalogService;
            _dashboardService = dashboardService;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cooperativeService = cooperativeService;
        }
        #endregion

        #region Public Methods
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "FileDownload/";
            ViewBag.EntityTypeList = new List<SelectListItem>();
            ViewBag.EntityCodeList = new List<SelectListItem>();
            ViewBag.BaseDirectory = Convert.ToBase64String(Encoding.UTF8.GetBytes(_valueCatatalogService.GetByCode(ConstantsValueCatalogs.RutaBaseDictorioArchivos).Name));

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

        [HttpGet]
        public IActionResult GetDirectoriesOrFiles(string entityCode, string directory)
        {
            var lst = _fileDownloadService.GetDirectoriesFromBaseDirectory(entityCode, directory);
            return Json(lst);
        }

        [HttpPost]
        public IActionResult DownloadFile(string pathFile)
        {
            byte[] fileContent = _fileDownloadService.DownloadFile(pathFile);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(Path.GetFileName(pathFile), out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(fileContent, contentType, Path.GetFileName(pathFile));
        }

        #endregion
    }
}
