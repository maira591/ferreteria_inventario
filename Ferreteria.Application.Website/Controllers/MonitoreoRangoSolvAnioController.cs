using AutoMapper;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Monitoreo.RangoSolvAnio;
using Core.Domain.Monitoreo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoRangoSolvAnioController : Controller
    {
        #region Private Fields
        private readonly IMapper _mapper;
        private readonly IRangoSolvAnioService _rangoSolvAnioService;
        #endregion

        #region Constructor
        public MonitoreoRangoSolvAnioController(IMapper mapper, IRangoSolvAnioService rangoSolvAnioService)
        {
            _mapper = mapper;
            _rangoSolvAnioService = rangoSolvAnioService;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<RangoSolvAnioVM>, List<RangoSolvAnioModel>>((await _rangoSolvAnioService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Rango Solv Año";
            var year = DateTime.Now.Year;
            var model = new RangoSolvAnioModel() { Anio = year };
            var previousRange = await _rangoSolvAnioService.GetByYearAsync(year - 1);
            if (previousRange != null)
            {
                model.PreviousPattec1 = previousRange.Pattec1;
                model.PreviousPattec2 = previousRange.Pattec2;
                model.PreviousPattec3 = previousRange.Pattec3;
            }
            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Rango Solv Año";

                var rangoSolvAnio = await _rangoSolvAnioService.GetByIdAsync(int.Parse(id));
                var model = _mapper.Map<RangoSolvAnioModel>(rangoSolvAnio);
                var previousRange = await _rangoSolvAnioService.GetByYearAsync(model.Anio - 1);

                model.Ipc = model.Ipc.Replace(",", ".");
                model.Pattec1 = model.Pattec1.Replace(",", ".");
                model.Pattec2 = model.Pattec2.Replace(",", ".");
                model.Pattec3 = model.Pattec3.Replace(",", ".");

                if (previousRange != null)
                {
                    model.PreviousPattec1 = previousRange.Pattec1.Replace(",", ".");
                    model.PreviousPattec2 = previousRange.Pattec2.Replace(",", ".");
                    model.PreviousPattec3 = previousRange.Pattec3.Replace(",", ".");
                }
                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(RangoSolvAnioModel model)
        {
            try
            {
                await _rangoSolvAnioService.AddOrUpdateAsync(_mapper.Map<RangoSolvAnioVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Rango Solv Año guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(RangoSolvAnioModel model)
        {
            model = FormatValues(model);
            model.Ipc = Common.DecimalValue(model.Ipc);

            string message = await _rangoSolvAnioService.Validations(_mapper.Map<RangoSolvAnioVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public JsonResult CalculatePattec([FromBody] RangoSolvAnioModel model)
        {
            model = FormatValues(model);

            var result = _rangoSolvAnioService.CalculatePattec(_mapper.Map<RangoSolvAnioVM>(model));

            var newModel = FormatValues(_mapper.Map<RangoSolvAnioModel>(result));
            newModel.Pattec1 = newModel.Pattec1.Replace(",", ".");
            newModel.Pattec2 = newModel.Pattec2.Replace(",", ".");
            newModel.Pattec3 = newModel.Pattec3.Replace(",", ".");
            return Json(newModel);
        }
        #endregion

        #region Private Methods
        private static RangoSolvAnioModel FormatValues(RangoSolvAnioModel model)
        {
            model.Pattec1 = model.Pattec1 != null ? Common.DecimalValue(model.Pattec1) : "";
            model.Pattec2 = model.Pattec2 != null ? Common.DecimalValue(model.Pattec2) : "";
            model.Pattec3 = model.Pattec3 != null ? Common.DecimalValue(model.Pattec3) : "";
            model.PreviousPattec1 = model.PreviousPattec1 != null ? Common.DecimalValue(model.PreviousPattec1) : "";
            model.PreviousPattec2 = model.PreviousPattec2 != null ? Common.DecimalValue(model.PreviousPattec2) : "";
            model.PreviousPattec3 = model.PreviousPattec3 != null ? Common.DecimalValue(model.PreviousPattec3) : "";
            model.Ipc = model.Ipc != null ? Common.DecimalValue(model.Ipc) : "";

            return model;
        }

        #endregion
    }
}
