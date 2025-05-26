using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.FormatService;
using Core.Domain.LoadTypeService;
using Core.Domain.MaximunDateService;
using Core.Domain.OfficeService;
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
    public class MaximunDateController : Controller
    {
        #region Private Region
        private readonly IMaximunDateService _maximunDateService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IFormatService _formatService;
        private readonly ILoadTypeService _loadTypeService;
        private readonly IOfficeService _officeService;
        #endregion

        #region Constructor
        public MaximunDateController(IMaximunDateService maximunDateService, IMapper mapper, IFormatService formatService, IUserBasicModel userBasicModel,
            ILoadTypeService loadTypeService, IOfficeService officeService)
        {
            _maximunDateService = maximunDateService;
            _formatService = formatService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _loadTypeService = loadTypeService;
            _officeService = officeService;
        }
        #endregion


        #region Public Methods
        public ActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> IndexGrid()
        {
            List<MaximunDateModel> model = _mapper.Map<List<MaximunDateVM>, List<MaximunDateModel>>((await _maximunDateService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar fecha máxima de transmisión";
            MaximunDateVM maximunDate = await _maximunDateService.GetByIdAsync(new Guid(id));
            ViewBag.ListFormatTypes = await FormatTypeList();
            ViewBag.ListLoadType = await LoadTypeList(maximunDate.CooperativeTypeId);
            MaximunDateModel model = _mapper.Map<MaximunDateVM, MaximunDateModel>(maximunDate);
            model.GuidCooperativeTypeId = maximunDate.CooperativeTypeId.ToString();
            model.GuidLoadTypeId = maximunDate.LoadTypeId.ToString();
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(MaximunDateModel maximunDate)
        {
            string message = string.Empty;
            maximunDate.CooperativeTypeId = new Guid(maximunDate.GuidCooperativeTypeId);
            maximunDate.LoadTypeId = new Guid(maximunDate.GuidLoadTypeId);
            message = await _maximunDateService.Validations(_mapper.Map<MaximunDateVM>(maximunDate));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(MaximunDateModel maximunDate)
        {
            try
            {
                if (maximunDate.Id == Guid.Empty)
                {
                    maximunDate.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    maximunDate.CreatedOn = DateTime.Now;
                }
                else
                {
                    maximunDate.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    maximunDate.UpdatedOn = DateTime.Now;
                }

                maximunDate.CooperativeTypeId = new Guid(maximunDate.GuidCooperativeTypeId);
                maximunDate.LoadTypeId = new Guid(maximunDate.GuidLoadTypeId);
                await _maximunDateService.AddOrUpdateAsync(_mapper.Map<MaximunDateVM>(maximunDate));

                return Json(JsonResponseFactory.SuccessResponse("Fecha máxima de transmisión guardada correctamente."));
            }
            catch (Exception e)
            {
                return Json(JsonResponseFactory.ErrorResponse($"Ocurrió un error inesperado: {e.Message}."));
            }
        }

        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear fecha máxima de transmisión";
            ViewBag.ListFormatTypes = await FormatTypeList();
            ViewBag.ListLoadType = new List<SelectListItem>();
            return PartialView("_CreateOrEdit", new MaximunDateModel { CutoffDate = DateTime.Now, MaxDate = DateTime.Now, IsEnabled = true });
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _maximunDateService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó la fecha máxima de transmisión correctamente."));
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
        public async Task<ActionResult> ExportData(string fileName)
        {
            List<List<object>> sheetsData = new();
            List<string> sheetsName = new();

            var maximunDates = (await _maximunDateService.GetAllAsync()).ToList();
            var maximunDateData = maximunDates.Select(x => new
            {
                TipoCooperativa = x.Type.Name,
                TipoCarga = x.FkLoadType.Name,
                FechaCorte = x.CutoffDate.ToString("dd/MM/yyyy"),
                FechaMaximaTransmision = x.MaxDate.ToString("dd/MM/yyyy"),
                Activo = (x.IsEnabled ? "SI" : "NO")
            }).ToList<object>();
            sheetsData.Add(maximunDateData);
            sheetsName.Add("Fechas máximas de transmisión");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
        #endregion


        #region Private Methods

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

        [HttpGet]
        public async Task<ActionResult> GetEntityList(Guid id)
        {
            List<LoadTypeVM> entities = (await _loadTypeService.GetByCooperativeTypeAsync(id)).Where(x => x.IsEnabled).ToList();
            var entityList = Common.LoadList(entities, "Name", "Id");

            if (entityList == null)
            {
                return Json(new List<LoadTypeVM>());
            }

            return Json(entityList);
        }

        private async Task<List<SelectListItem>> LoadTypeList(Guid id)
        {
            List<LoadTypeVM> entities = (await _loadTypeService.GetByCooperativeTypeAsync(id)).ToList();
            var loadTypeListSelect = Common.LoadList(entities, "Name", "Id"); if (loadTypeListSelect == null)
            {
                return new List<SelectListItem>();
            }
            return loadTypeListSelect;
        }
        #endregion
    }
}
