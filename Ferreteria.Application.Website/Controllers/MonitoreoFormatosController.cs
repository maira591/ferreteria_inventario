using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.FormatosService;
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
    public class MonitoreoFormatosController : Controller
    {
        #region Private fields

        private readonly IFormatosService _formatosService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;

        #endregion


        #region Constructor
        public MonitoreoFormatosController(IFormatosService formatosService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _formatosService = formatosService;
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
            var model = _mapper.Map<List<FormatoVM>, List<FormatoModel>>((await _formatosService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear formato";
            ViewBag.TablesList = await GetTablesList();
            ViewBag.EntityAppliesList = await GetEntityAppliesList();
            return PartialView("_CreateOrEdit", new FormatoModel { Estado = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar formato";

                ViewBag.TablesList = await GetTablesList();
                ViewBag.EntityAppliesList = await GetEntityAppliesList();
                var formato = await _formatosService.GetByIdAsync(int.Parse(id));
                FormatoModel model = _mapper.Map<FormatoModel>(formato);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(FormatoModel model)
        {
            try
            {
                await _formatosService.AddOrUpdateAsync(_mapper.Map<FormatoVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Formato guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(FormatoModel model)
        {
            string message = string.Empty;

            message = await _formatosService.Validations(_mapper.Map<FormatoVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion


        private async Task<List<SelectListItem>> GetTablesList()
        {

            List<string> lstTables = await _formatosService.GetTableNames();
            var lstObject = lstTables.Select(x => new { Name = x, Id = x }).ToList();

            var selectList = Common.LoadList(lstObject, "Name", "Id");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }
        private async Task<List<SelectListItem>> GetEntityAppliesList()
        {
            List<ValueCatalogVM> lstEntityApplies = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.EntidadAplicaFormato)).ValueCatalogs;
            var lstObjetc = lstEntityApplies.Select(x => new { Id = x.Name, Name = x.Description }).ToList();
            var selectList = Common.LoadList(lstObjetc, "Name", "Id");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }
    }
}