using AutoMapper;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Monitoreo.ConstantesService;
using Core.Domain.Monitoreo.RangosValores;
using Core.Domain.Monitoreo.ViewModel;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoRangosValoresController : Controller
    {
        #region Private Fields
        private readonly IMapper _mapper;
        private readonly IRangosValoresService _rangosValoresService;
        private readonly IConstantesService _constantesService;
        #endregion

        #region Constructor
        public MonitoreoRangosValoresController(IMapper mapper, IRangosValoresService rangosValoresService, IConstantesService constantesService)
        {
            _mapper = mapper;
            _rangosValoresService = rangosValoresService;
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
            var model = _mapper.Map<List<RangosValoresVM>, List<RangosValoresModel>>((await _rangosValoresService.GetAllAsync()).ToList());

            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Rangos Valores";
            ViewBag.TiposRangos = await RangeTypesList(ConstantsMonitoreo.ConstantRangeTypes);
            var model = new RangosValoresModel() { };

            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Rangos Valores";

                var rangoValores = await _rangosValoresService.GetByIdAsync(int.Parse(id));
                var model = _mapper.Map<RangosValoresModel>(rangoValores);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(RangosValoresModel model)
        {
            try
            {
                await _rangosValoresService.AddOrUpdateAsync(_mapper.Map<RangosValoresVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Rango Valor guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(RangosValoresModel model)
        {
            string message = await _rangosValoresService.Validations(_mapper.Map<RangosValoresVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<JsonResult> GetLastValues(string rangeType)
        {
            long value = await _rangosValoresService.GetLasValue(rangeType);

            return new JsonResult(value);
        }

        [HttpPost]
        public JsonResult CalculatePattec([FromBody] RangosValoresModel model)
        {
            return Json(new { });
        }
        #endregion

        #region Private Methods
        public async Task<List<SelectListItem>> RangeTypesList(string code, int id = 0)
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


    }
}
