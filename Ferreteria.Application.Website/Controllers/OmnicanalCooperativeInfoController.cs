using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.CooperativeService;
using Core.Domain.Monitoreo.PucSolidariaService;
using Core.Domain.Monitoreo.ViewModel;
using Core.Domain.Utils;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class OmnicanalCooperativeInfoController : Controller
    {
        #region Private fields

        private readonly IPucSolidariaService _pucSolidariaService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;
        private readonly ICooperativeService _cooperativeService;

        #endregion

        #region Constructor
        public OmnicanalCooperativeInfoController(IPucSolidariaService pucFinancieraService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService,
            ICooperativeService cooperativeService)
        {
            _pucSolidariaService = pucFinancieraService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
            _cooperativeService = cooperativeService;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Actualizar información del usuario
        /// </summary>
        /// <returns></returns>        
        public ActionResult EditInfo()
        {
            return View("Edit", new CooperativeInfoModel());
        }

        [HttpPost]
        public JsonResult UpdateInfoUser(CooperativeInfoModel model)
        {
            return Json(null);
        }
        #endregion

        #region Private Methods
        private List<SelectListItem> GetPucSignsAccountsList()
        {
            List<SelectListItem> selectList = new()
            {
                new() { Text = "Positiva (+)", Value = "+" },
                new() { Text = "Negativa (-)", Value = "-" },
                new() { Text = "Doble (*)", Value = "*" }
            };

            return selectList;
        }
        #endregion

    }
}