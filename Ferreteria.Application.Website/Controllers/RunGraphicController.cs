using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.CooperativeService;
using Core.Domain.GraphicGroupService;
using Core.Domain.GraphicIndicatorService;
using Core.Domain.GraphicSubGroupService;
using Core.Domain.IndicatorService;
using Core.Domain.OfficeService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.Utils;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminSIG, PrivilegesEnum.AdminCore, PrivilegesEnum.ViewGraphicsSIG)]
    public class RunGraphicController : Controller
    {
        #region Private fields
        private readonly IGraphicService _graphicService;
        private readonly IGraphicGroupService _graphicGroupService;
        private readonly IGraphicSubGroupService _graphicSubGroupService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IIndicatorService _indicatorService;
        private readonly ICatalogService _catalogService;
        private readonly IGraphicIndicatorService _graphicIndicatorService;
        private readonly IOfficeService _officeService;
        private readonly IDashboardService _dashboardService;
        private readonly ICooperativeService _cooperativeService;
        private readonly UserBasicModel _currentUser;
        #endregion

        #region Constructor
        public RunGraphicController(IGraphicService graphicService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService,
                                IGraphicGroupService graphicGroupService, IGraphicSubGroupService graphicSubGroupService, IIndicatorService indicatorService,
                                IGraphicIndicatorService graphicIndicatorService, IOfficeService officeService, IDashboardService dashboardService,
                                ICooperativeService cooperativeService
                                )
        {
            _graphicService = graphicService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
            _graphicGroupService = graphicGroupService;
            _graphicSubGroupService = graphicSubGroupService;
            _indicatorService = indicatorService;
            _graphicIndicatorService = graphicIndicatorService;
            _officeService = officeService;
            _dashboardService = dashboardService;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cooperativeService = cooperativeService;
        }
        #endregion

        #region Public Methods
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.ListGraphicGroup = GetListGraphicGroup().Result;
            ViewBag.ListGraphicPeriodicty = GetListGraphicPeriodicty().Result;
            ViewBag.EntityTypeList = new List<SelectListItem>();
            ViewBag.EntityCodeList = new List<SelectListItem>();

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
        public async Task<JsonResult> GetSubGroups(Guid groupId)
        {
            List<GraphicSubGroupVM> lst = (await _graphicSubGroupService.GetAllByGraphicGroupIdAsync(groupId)).ToList();
            var lstObject = lst.Select(x => new { Name = x.Name, Id = x.Id }).ToList();

            if (lstObject == null)
            {
                return Json(new());
            }

            return Json(lstObject);
        }

        [HttpPost]
        public async Task<IActionResult> ExcecuteGraphics(RunGraphicModel graphicModel)
        {
            try
            {
                var user = _mapper.Map<UserModelComplete>(_userBasicModel.GetCurrentUser());
                var listGraphics = await _graphicService.GenerateGraphicSIG(user.Roles, graphicModel.StartDate, graphicModel.EndDate, int.Parse(graphicModel.EntityCode), int.Parse(graphicModel.EntityType), graphicModel.GraphicGroupId, graphicModel.GraphicSubGroupId, graphicModel.PeriodicityId, graphicModel.IndicatorsSiglas);

                return Json(listGraphics);
            }
            catch (Exception e)
            {
                return Json(new { Valid = false, Message = "Ocurrió un error inesperado." });
            }
        }


        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> GetListGraphicGroup()
        {
            List<GraphicGroupVM> groups = (await _graphicGroupService.GetAllAsync()).ToList();
            var list = Common.LoadList(groups, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> GetListGraphicPeriodicty()
        {
            List<ValueCatalogVM> groups = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.PeriodicidadGraficas)).ValueCatalogs;
            var list = Common.LoadList(groups, "Code", "Id");

            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }
        #endregion


    }
}
