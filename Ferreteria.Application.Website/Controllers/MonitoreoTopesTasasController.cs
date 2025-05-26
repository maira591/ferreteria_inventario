using AutoMapper;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Monitoreo.ConstantesService;
using Core.Domain.Monitoreo.TopesTasasService;
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
    public class MonitoreoTopesTasasController : Controller
    {
        #region Private Fields
        private readonly IMapper _mapper;
        private readonly ITopesTasasService _topesTasasService;
        private readonly IConstantesService _constantesService;
        #endregion

        #region Constructor
        public MonitoreoTopesTasasController(IMapper mapper, ITopesTasasService topesTasasService, IConstantesService constantesService)
        {
            _mapper = mapper;
            _topesTasasService = topesTasasService;
            _constantesService = constantesService;
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
            var model = _mapper.Map<List<TopesTasasVM>, List<TopesTasasModel>>((await _topesTasasService.GetAllAsync()).ToList());

            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Topes Tasas";
            var clasifications = new List<SelectListItem>();
            clasifications.Add(new SelectListItem { Selected = true, Text = "Seleccione", Value = "" });
            clasifications.AddRange(await ClasificationList(ConstantsMonitoreo.ConstantClasification));

            ViewBag.Clasifications = clasifications;
            var model = new TopesTasasModel() { };
            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Topes Tasas";

                var rangoValores = await _topesTasasService.GetByIdAsync(id);
                var model = _mapper.Map<TopesTasasModel>(rangoValores);
                List<ConstantesVM> constantes = new List<ConstantesVM>();
                ConstantesVM constante = await _constantesService.GetByIdAsync(model.Clasificacion);
                constantes.Add(constante);
                ViewBag.Clasifications = Common.LoadList(constantes, "Nombre", "Id");
                model.TasaUsura = model.TasaUsura.Replace(",", ".");
                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(TopesTasasModel model)
        {
            try
            {
                model.TasaUsura = model.TasaUsura.Replace(",", ".");
                await _topesTasasService.AddOrUpdateAsync(_mapper.Map<TopesTasasVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Tope Tasa guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        public JsonResult GetLastDayOfMonth(DateTime fecIni)
        {
            string fecFin = LastDayOfDate(fecIni);
            return new JsonResult(fecFin);
        }

        [HttpPost]
        public async Task<JsonResult> GetLastDayOfMonthByClasification(int idClasificacion)
        {
            TopesTasasVM model = await _topesTasasService.GetLastEndDateByClasificationAsync(idClasificacion);
            var fecFin = string.Empty;
            var fecIni = string.Empty;
            if (model != null)
            {
                model.FecIni = model.FecFin.AddDays(1);
                fecIni = model.FecIni.ToString("dd/MM/yyyy");
                fecFin = LastDayOfDate(model.FecIni);
            }

            return new JsonResult(new { fecIni, fecFin });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(TopesTasasModel model)
        {
            string message = await _topesTasasService.Validations(_mapper.Map<TopesTasasVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }


        [HttpPost]
        public JsonResult CalculatePattec([FromBody] TopesTasasModel model)
        {
            return Json(new { });
        }
        #endregion

        #region Private Methods
        public async Task<List<SelectListItem>> ClasificationList(string code, int id = 0)
        {

            List<ConstantesVM> constantes = (await _constantesService.GetByParentCode(code)).OrderBy(x => x.Nombre).ToList();

            if (!id.Equals(0))
            {
                constantes.RemoveAll(t => t.Id != id);
            }

            var list = Common.LoadList(constantes, "Nombre", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        #endregion

        #region Private Methods
        private string LastDayOfDate(DateTime fecIni)
        {
            int dayFecFin = DateTime.DaysInMonth(fecIni.Year, fecIni.Month);
            return $"{dayFecFin}/{fecIni:MM}/{fecIni.Year}";
        }
        #endregion
    }
}
