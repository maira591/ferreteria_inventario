using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Recaudo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Recaudo.TasaMoraService;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class RecaudoTasaMoraController : Controller
    {

        #region Private fields

        private readonly ITasaMoraService _tasaMoraService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;

        #endregion

        #region Constructor
        public RecaudoTasaMoraController(ITasaMoraService tasaMoraService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _tasaMoraService = tasaMoraService;
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
            var model = _mapper.Map<List<TasaMoraVM>, List<TasaMoraModel>>((await _tasaMoraService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Tasa Mora";
            TasaMoraVM lastRecord = await _tasaMoraService.GetLastRecord();

            TasaMoraModel model = new()
            {
                FechaInicio = lastRecord.FechaFinal.AddDays(1),
                FechaFinal = lastRecord.FechaFinal.AddDays(1).AddMonths(1).AddDays(-1),
                IsNew = ConstantsMasterTable.IsCreate
            };

            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string fechaInicio, string fechaFinal)
        {
            try
            {
                ViewBag.TitleModal = "Editar Tasa Mora";

                var formato = await _tasaMoraService.GetByDates(Utilities.DateParse(fechaInicio).Value, Utilities.DateParse(fechaFinal).Value);
                TasaMoraModel model = _mapper.Map<TasaMoraModel>(formato);
                model.ValorTasaMora = model.ValorTasaMora.Replace(",", ".");

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(TasaMoraModel model)
        {
            try
            {
                model.ValorTasaMora = model.ValorTasaMora.Replace(",", ".");
                await _tasaMoraService.AddOrUpdateAsync(_mapper.Map<TasaMoraVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Tasa mora guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }
        #endregion

    }
}