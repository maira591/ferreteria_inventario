using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.TasasService;
using Core.Domain.Monitoreo.ViewModel;
using Core.Domain.Utils;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoTasasController : Controller
    {

        #region Private fields
        private readonly ITasasService _tasasService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;
        #endregion

        #region Constructor
        public MonitoreoTasasController(ITasasService formatosService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _tasasService = formatosService;
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
            ViewBag.NamesRates = await GetNamesRates();
            var model = _mapper.Map<List<TasasVM>, List<TasasModel>>((await _tasasService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear tasa";
            ViewBag.NamesRates = await GetNamesRates();

            return PartialView("_CreateOrEdit", new TasasModel() { });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar tasa";
                ViewBag.NamesRates = await GetNamesRates();

                var tasa = await _tasasService.GetByIdAsync(int.Parse(id));
                TasasModel model = _mapper.Map<TasasModel>(tasa);
                model.ValorTasa = model.ValorTasa.Replace(",", ".");

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetLastRecordByNameRates(string nameRates)
        {
            try
            {
                var tasa = _tasasService.GetLastRecordByNameRatesAsync(nameRates).Result;
                return Json(new { FechaTasa = tasa.FechaTasa.ToString("dd/MM/yyyy") });
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(TasasModel model)
        {
            try
            {
                model.ValorTasa = model.ValorTasa.Replace(",", ".");
                await _tasasService.AddOrUpdateAsync(_mapper.Map<TasasVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Formato guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> GetNamesRates()
        {
            List<ValueCatalogVM> lstEntityApplies = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.NombresTasasMonitoreo)).ValueCatalogs;
            var selectList = Common.LoadList(lstEntityApplies, "Name", "Code");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }
        #endregion

    }
}