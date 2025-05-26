using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.FormatService;
using Core.Domain.LoadTypeService;
using Core.Domain.OfficeService;
using Core.Domain.ViewModel;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class LoadTypeController : Controller
    {
        #region Private Fields
        private readonly ILoadTypeService _loadTypeService;
        private readonly string _apitoken;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IFormatService _formatService;
        private readonly IMapper _mapper;
        private readonly IOfficeService _officeService;
        #endregion

        #region Constructor
        public LoadTypeController(IConfigurator configurationService, ILoadTypeService loadTypeService, IUserBasicModel userBasicModel, IFormatService formatService, IMapper mapper, IOfficeService officeService)
        {
            _loadTypeService = loadTypeService;
            var cfgService = configurationService;
            _apitoken = cfgService.GetKey("ApiToken");
            _userBasicModel = userBasicModel;
            _formatService = formatService;
            _mapper = mapper;
            _officeService = officeService;
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

            var model = _mapper.Map<List<LoadTypeVM>, List<LoadTypeModel>>((await _loadTypeService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Tipo de Carga";
            ViewBag.PeriodicityList = await PeriodicityList();
            ViewBag.ListFormatTypes = await FormatTypeList();
            return PartialView("_CreateOrEdit", new LoadTypeModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar Tipo de Carga";
            var loadType = await _loadTypeService.GetByIdAsync(new Guid(id));
            ViewBag.PeriodicityList = await PeriodicityList();
            ViewBag.ListFormatTypes = await FormatTypeList();
            LoadTypeModel model = _mapper.Map<LoadTypeModel>(loadType);
            model.GuidCooperativeTypeId = loadType.CooperativeTypeId.ToString();
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(LoadTypeModel loadType)
        {
            try
            {
                loadType.AppId = new Guid(_apitoken);
                if (loadType.Id == Guid.Empty)
                {
                    loadType.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    loadType.CreatedOn = DateTime.Now;
                }
                else
                {
                    loadType.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    loadType.UpdatedOn = DateTime.Now;
                }

                loadType.CooperativeTypeId = new Guid(loadType.GuidCooperativeTypeId);
                await _loadTypeService.AddOrUpdateAsync(_mapper.Map<LoadTypeVM>(loadType));

                return Json(JsonResponseFactory.SuccessResponse("Tipo de Carga guardada correctamente."));
            }
            catch (Exception e)
            {
                var err = e.Message;
                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El nombre ingresado ya existe y está siendo utilizado por otro Tipo de Carga."));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _loadTypeService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el Tipo de Carga correctamente."));
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
        public async Task<JsonResult> Validations(LoadTypeModel loadType)
        {
            string message = await _loadTypeService.Validations(_mapper.Map<LoadTypeVM>(loadType));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> ExportLoadTypes(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();
            var loadTypes = (await _loadTypeService.GetAllAsync()).ToList();

            var newLoadTypes = (from loadType in loadTypes select new { Nombre = loadType.Name, TipoFormato = loadType.Type.Name, Periodicidad = loadType.Periodicity.Name, Activo = loadType.IsEnabled ? "SI" : "NO" }).OrderBy(t => t.Nombre).ToList<object>();
            sheetsData.Add(newLoadTypes);
            sheetsName.Add("Tipos de Carga");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> PeriodicityList()
        {
            List<PeriodicityVM> periodicitys = await _loadTypeService.GetPeriodicities();
            var periodicitiesList = Common.LoadList(periodicitys, "Name", "Id");
            if (periodicitiesList == null)
            {
                return new List<SelectListItem>();
            }
            return periodicitiesList;
        }

        private async Task<List<SelectListItem>> FormatTypeList()
        {
            List<ValueCatalogVM> formatTypes = await _formatService.GetFormatTypes();
            var formatTypeList = Common.LoadList(formatTypes, "Name", "Id");
            if (formatTypeList == null)
            {
                return new List<SelectListItem>();
            }
            return formatTypeList;

        }
        #endregion
    }
}
