using AutoMapper;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.MatcamelBetasService;
using Core.Domain.Monitoreo.ViewModel;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
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
    public class MonitoreoMatcamelBetasController : Controller
    {
        #region Private Fields
        private readonly IMatcamelBetasService _matcamelBetasService;
        private readonly IMapper _mapper;
        private readonly ICatalogService _catalogService;
        private readonly IValueCatalogService _valueCatalogService;
        #endregion

        #region Constructor
        public MonitoreoMatcamelBetasController(IMatcamelBetasService matcamelBetasService, IMapper mapper, ICatalogService catalogService, IValueCatalogService valueCatalogService)
        {
            _matcamelBetasService = matcamelBetasService;
            _mapper = mapper;
            _catalogService = catalogService;
            _valueCatalogService = valueCatalogService;
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
            var model = _mapper.Map<List<MatcamelBetasVM>, List<MatcamelBetasModel>>((await _matcamelBetasService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Matcamel Betas";
            ViewBag.ListBetas = await BetasList();
            ViewBag.EntityList = await GetEntityAppliesList();
            var year = DateTime.Now.Year;
            return PartialView("_CreateOrEdit", new MatcamelBetasModel() { Anio = year });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Capital Requerido";

                var matcamelBetas = await _matcamelBetasService.GetByIdAsync(int.Parse(id));
                MatcamelBetasModel model = _mapper.Map<MatcamelBetasModel>(matcamelBetas);
                ViewBag.ListBetas = await BetasList(model.IdBeta);
                ViewBag.EntityList = await GetEntityAppliesList(model.TipoEntidad.ToString());
                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(MatcamelBetasModel model)
        {
            try
            {
                await _matcamelBetasService.AddOrUpdateAsync(_mapper.Map<MatcamelBetasVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Matcamel Betas guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(MatcamelBetasModel model)
        {
            model.Valor = Common.DecimalValue(model.Valor);
            string message = await _matcamelBetasService.Validations(_mapper.Map<MatcamelBetasVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion


        #region Private Methods
        public async Task<List<SelectListItem>> BetasList(string id = "")
        {
            List<ValueCatalogVM> betas = (await _valueCatalogService.GetValueCatalogsWithCatalogCodeAsync(ConstantsCatalogs.BetasMonitoreo)).OrderBy(x => x.Code).ToList();
            if (!id.Equals(string.Empty))
            {
                betas.RemoveAll(t => t.Code != id);
            }

            var list = Common.LoadList(betas, "Name", "Code");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> GetEntityAppliesList(string tipoEntidad = "")
        {
            List<ValueCatalogVM> lstEntityApplies = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.EntidadAplicaFormato)).ValueCatalogs;
            if (!tipoEntidad.Equals(string.Empty))
            {
                lstEntityApplies.RemoveAll(t => t.Name != tipoEntidad);
            }

            var lstObjetc = lstEntityApplies.Select(x => new { Id = x.Name, Name = x.Description }).ToList();
            var selectList = Common.LoadList(lstObjetc, "Name", "Id");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }
        #endregion
    }
}
