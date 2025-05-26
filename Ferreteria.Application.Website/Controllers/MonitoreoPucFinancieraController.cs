using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.PucFinancieraService;
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
    public class MonitoreoPucFinancieraController : Controller
    {
        #region Private fields

        private readonly IPucFinancieraService _pucFinancieraService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;

        #endregion

        #region Constructor
        public MonitoreoPucFinancieraController(IPucFinancieraService pucFinancieraService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _pucFinancieraService = pucFinancieraService;
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
            var model = _mapper.Map<List<CuentaPucFinanVM>, List<CuentaPucFinanModel>>((await _pucFinancieraService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear Cuenta";
            ViewBag.PucSignsAccountsList = GetPucSignsAccountsList();
            return PartialView("_CreateOrEdit", new CuentaPucFinanModel() { Codmon = true, IsNew = ConstantsMasterTable.IsCreate });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string account, string dateStart)
        {
            try
            {
                ViewBag.TitleModal = "Editar Cuenta";
                ViewBag.PucSignsAccountsList = GetPucSignsAccountsList();
                var cuenta = await _pucFinancieraService.GetByAccountAsync(account, Utilities.DateParse(dateStart).Value);
                CuentaPucFinanModel model = _mapper.Map<CuentaPucFinanModel>(cuenta);

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
                var dateEnd = _pucFinancieraService.GetLastRecordByAccount(account).Result;
                return Json(new { FechaInicio = dateEnd.HasValue ? dateEnd.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"), Exists = dateEnd.HasValue });
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(CuentaPucFinanModel model)
        {
            try
            {
                await _pucFinancieraService.AddOrUpdateAsync(_mapper.Map<CuentaPucFinanVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Cuenta guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(CuentaPucFinanModel model)
        {
            string message = string.Empty;

            message = await _pucFinancieraService.Validations(_mapper.Map<CuentaPucFinanVM>(model));

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