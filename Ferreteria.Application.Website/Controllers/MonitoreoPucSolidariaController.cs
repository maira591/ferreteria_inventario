using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.PucSolidariaService;
using Core.Domain.Monitoreo.ViewModel;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoPucSolidariaController : Controller
    {
        #region Private fields

        private readonly IPucSolidariaService _pucSolidariaService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;

        #endregion

        #region Constructor
        public MonitoreoPucSolidariaController(IPucSolidariaService pucFinancieraService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _pucSolidariaService = pucFinancieraService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
        }
        #endregion

        #region Public Methods
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<CuentaPucSolidVM>, List<CuentaPucSolidModel>>((await _pucSolidariaService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear Cuenta";
            ViewBag.PucSignsAccountsList = GetPucSignsAccountsList();
            return PartialView("_CreateOrEdit", new CuentaPucSolidModel() { Codmon = true, IsNew = ConstantsMasterTable.IsCreate });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string account, string dateStart)
        {
            try
            {
                ViewBag.TitleModal = "Editar Cuenta";
                ViewBag.PucSignsAccountsList = GetPucSignsAccountsList();
                var cuenta = await _pucSolidariaService.GetByAccountAsync(account, Utilities.DateParse(dateStart).Value);
                CuentaPucSolidModel model = _mapper.Map<CuentaPucSolidModel>(cuenta);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetLastRecordByAccount(string account)
        {
            try
            {
                var dateEnd = _pucSolidariaService.GetLastRecordByAccount(account).Result;
                return Json(new { FechaInicio = dateEnd.HasValue ? dateEnd.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"), Exists = dateEnd.HasValue });
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(CuentaPucSolidModel model)
        {
            try
            {
                await _pucSolidariaService.AddOrUpdateAsync(_mapper.Map<CuentaPucSolidVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Cuenta guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(CuentaPucSolidModel model)
        {
            string message = string.Empty;

            message = await _pucSolidariaService.Validations(_mapper.Map<CuentaPucSolidVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
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