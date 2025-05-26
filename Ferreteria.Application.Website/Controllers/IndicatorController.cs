using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.IndicatorService;
using Core.Domain.OfficeService;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class IndicatorController : Controller
    {
        #region Private Fields
        private readonly IIndicatorService _indicatorService;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IMapper _mapper;
        private readonly IOfficeService _officeService;
        private readonly IValueCatalogService _valueCatalogService;

        #endregion

        #region Constructor
        public IndicatorController(IIndicatorService indicatorService, IUserBasicModel userBasicModel,
                                   IMapper mapper, IOfficeService officeService, IValueCatalogService valueCatalogService)
        {
            _indicatorService = indicatorService;
            _userBasicModel = userBasicModel;
            _mapper = mapper;
            _officeService = officeService;
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

            var model = _mapper.Map<List<IndicatorVM>, List<IndicatorModel>>((await _indicatorService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Indicador";

            ViewBag.ListIndicatorGroup = (await _valueCatalogService.GetValueCatalogsWithCatalogCodeAsync(ConstantsCatalogs.GrupoIndicadores)).ToList().ToLoadGenericList("Name", "Id");

            return PartialView("_CreateOrEdit", new IndicatorModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar Indicador";
            var indicator = await _indicatorService.GetByIdAsync(new Guid(id));

            ViewBag.ListIndicatorGroup = (await _valueCatalogService.GetValueCatalogsWithCatalogCodeAsync(ConstantsCatalogs.GrupoIndicadores)).ToList().ToLoadGenericList("Name", "Id");

            IndicatorModel model = _mapper.Map<IndicatorModel>(indicator);

            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(IndicatorModel indicator)
        {
            try
            {

                if (indicator.Id == Guid.Empty)
                {
                    indicator.CreatedBy = _userBasicModel.GetCurrentUser().UserName;
                    indicator.CreatedOn = DateTime.Now;
                    indicator.IsNew = true;
                }
                else
                {
                    indicator.UpdatedBy = _userBasicModel.GetCurrentUser().UserName;
                    indicator.UpdatedOn = DateTime.Now;
                }

                await _indicatorService.AddOrUpdateAsync(_mapper.Map<IndicatorVM>(indicator));

                return Json(JsonResponseFactory.SuccessResponse("Información guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _indicatorService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Información guardada correctamente."));
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("||") >= 0)
                {
                    return Json(JsonResponseFactory.ErrorResponse(ex.Message.Replace("||", "")));
                }
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(IndicatorModel indicator)
        {
            string message = await _indicatorService.Validations(_mapper.Map<IndicatorVM>(indicator));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> ExportIndicator(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();
            var loadTypes = (await _indicatorService.GetAllAsync()).ToList();

            var newLoadTypes = (from indicator in loadTypes select new { Grupo = indicator.ValueCatalogGroupIndicator.Name, Nombre = indicator.Name, Sigla = indicator.Sigla, Activo = indicator.IsEnabled ? "SI" : "NO" }).OrderBy(t => t.Grupo).ToList<object>();
            sheetsData.Add(newLoadTypes);
            sheetsName.Add("Indicadores");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }

        #endregion



    }
}
