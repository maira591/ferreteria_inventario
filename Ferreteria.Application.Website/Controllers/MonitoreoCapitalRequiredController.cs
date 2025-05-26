using AutoMapper;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Monitoreo.CapitalRequeridoService;
using Core.Domain.Monitoreo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoCapitalRequiredController : Controller
    {
        #region Private Fields
        private readonly ICapitalRequeridoService _capitalRequeridoService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public MonitoreoCapitalRequiredController(ICapitalRequeridoService capitalRequeridoService, IMapper mapper)
        {
            _capitalRequeridoService = capitalRequeridoService;
            _mapper = mapper;
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
            var model = _mapper.Map<List<CapitalRequeridoVM>, List<CapitalRequeridoModel>>((await _capitalRequeridoService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Capital Requerido";
            var year = await _capitalRequeridoService.GetNextYear();
            return PartialView("_CreateOrEdit", new CapitalRequeridoModel() { Anio = year });
        }
        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Capital Requerido";

                var capitalRequerido = await _capitalRequeridoService.GetByIdAsync(int.Parse(id));
                CapitalRequeridoModel model = _mapper.Map<CapitalRequeridoModel>(capitalRequerido);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(CapitalRequeridoModel model)
        {
            try
            {
                await _capitalRequeridoService.AddOrUpdateAsync(_mapper.Map<CapitalRequeridoVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Capital Requerido guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(CapitalRequeridoModel model)
        {
            model.Ipc = Common.DecimalValue(model.Ipc);
            model.CapitalFinanciera = Common.DecimalValue(model.CapitalFinanciera);
            model.CapitalSolidaria = Common.DecimalValue(model.CapitalSolidaria);
            string message = await _capitalRequeridoService.Validations(_mapper.Map<CapitalRequeridoVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion
    }
}
