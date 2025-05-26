using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CooperativeService;
using Core.Domain.FileDownloadService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.Recaudo.AutomaticLogCalculationService;
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
    [Auth(PrivilegesEnum.AdminCore, PrivilegesEnum.AutomaticCalculation)]
    public class RecaudoSettlementCalculationController : Controller
    {
        #region Private fields
        private readonly IAutomaticCalculationService _automaticLogCalculationService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IValueCatalogService _valueCatatalogService;
        private readonly ICooperativeService _cooperativeService;
        private readonly IDashboardService _dashboardService;
        private readonly UserBasicModel _currentUser;
        #endregion

        #region Constructor
        public RecaudoSettlementCalculationController(IAutomaticCalculationService automaticLogCalculationService, IMapper mapper, IUserBasicModel userBasicModel, IValueCatalogService valueCatatalogService, IDashboardService dashboardService, ICooperativeService cooperativeService)
        {
            _automaticLogCalculationService = automaticLogCalculationService;
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
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "RecaudoSettlementCalculation/";

            return View();
        }

        [HttpPost]
        public IActionResult RunCalculation(string cutoffDate)
        {
            var lst = _automaticLogCalculationService.RunSettlementCalculation(cutoffDate);
            return Json(lst);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateExistsSettlementCalculation(string cutoffDate)
        {
            var reponse = await _automaticLogCalculationService.ValidateExistsSettlementCalculation(cutoffDate);
            return Json(reponse);
        }
        #endregion

        #region Private Methods
        private List<SelectListItem> GetCalculationType()
        {
            return new List<SelectListItem>()
            {
                new()
                {
                        Text = "Recálculo Todas",
                        Value = "false",
                        Selected =  true
                },
                new()
                {
                        Text = "Recálculo Individual",
                        Value = "true"
                }
            };
        }

        #endregion
    }

}
