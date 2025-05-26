using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CooperativeService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly ICooperativeService _cooperativeService;
        private readonly IDashboardService _dashboardService;
        private readonly IUserBasicModel _userBasicModel;
        private readonly UserBasicModel _currentUser;
        private readonly IMapper _mapper;

        public DashBoardController(IUserBasicModel userBasicModel, ICooperativeService cooperativeService,
                                   IDashboardService dashboardService, IMapper mapper)
        {
            _userBasicModel = userBasicModel;
            _currentUser = _userBasicModel.GetCurrentUser();
            _cooperativeService = cooperativeService;
            _dashboardService = dashboardService;
            _mapper = mapper;
        }

        [Auth(PrivilegesEnum.ViewDashboard)]
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
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


        public async Task<PartialViewResult> GetDashboard(string cutOffDate, int entityId, int entityType)
        {
            DashBoardModel model = _mapper.Map<DashBoardModel>(await _dashboardService.GenerateGraphics(cutOffDate, entityId, entityType));

            return PartialView("_Dashboard", model);
        }
    }
}
