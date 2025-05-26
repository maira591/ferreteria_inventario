using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.DataAccess.Model;
using Core.Domain.Recaudo.AutomaticLogCalculationService;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore, PrivilegesEnum.AutomaticCalculation)]
    public class RecaudoAutomaticCalculationController : Controller
    {

        #region Private Fields
        private readonly IHttpContextAccessor _accessor;
        private readonly IAutomaticCalculationService _automaticCalculationService;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor
        public RecaudoAutomaticCalculationController(IHttpContextAccessor accessor, IAutomaticCalculationService automaticLogCalculationService, IMapper mapper)
        {
            _accessor = accessor;
            _mapper = mapper;
            _automaticCalculationService = automaticLogCalculationService;
        }
        #endregion

        #region Public methods

        #endregion
        public ActionResult Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TotalRows = 0;
            return View();
        }

        [HttpGet]
        public void SetDates(string startDate, string endDate)
        {
            HttpContext.Session.SetString("LogStartDate", startDate);
            HttpContext.Session.SetString("LogEndDate", endDate);
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var startDate = _accessor.HttpContext.Session.GetString("LogStartDate");
            var endDate = _accessor.HttpContext.Session.GetString("LogEndDate");

            if (startDate == null)
            {
                return PartialView("_IndexGrid", new List<string>());
            }

            var logCalculations = await  _automaticCalculationService.GetByRangeDates(Utilities.DateParse(startDate).Value, Utilities.DateParse(endDate).Value);

           
            return PartialView("_IndexGrid", _mapper.Map<List<AutomaticLogCalculationModel>>(logCalculations) );
        }
    }
}
