using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.LogService;
using Core.Domain.OfficeService;
using Core.Domain.QueueMessagePriorityService;
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
    public class ProcessingOrderController : Controller
    {
        #region Private fields
        private readonly IQueueMessagePriorityService _queueMessagePriority;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ILogService _logService;
        private readonly IOfficeService _officeService;
        #endregion

        public ProcessingOrderController(IQueueMessagePriorityService queueMessagePriority, IMapper mapper, IUserBasicModel userBasicModel,
                                              ILogService logService, IOfficeService officeService)
        {
            _queueMessagePriority = queueMessagePriority;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _logService = logService;
            _officeService = officeService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<QueueMessagePriorityVM>, List<QueueMessagePriorityModel>>((await _queueMessagePriority.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.EntityTypeList = await EntityTypeList();
            ViewBag.EntityCodeList = new List<SelectListItem>();
            ViewBag.PriorityList = PeriodicityList();
            ViewBag.TitleModal = "Crear priorización cargue";
            return PartialView("_CreateOrEdit", new QueueMessagePriorityModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.EntityTypeList = await EntityTypeList();
            ViewBag.PriorityList = PeriodicityList();
            ViewBag.TitleModal = "Editar priorización cargue";
            var queuePriority = await _queueMessagePriority.GetByIdAsync(new Guid(id));
            QueueMessagePriorityModel model = _mapper.Map<QueueMessagePriorityModel>(queuePriority);
            ViewBag.EntityCodeList = await EntityList(queuePriority.EntityType);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(QueueMessagePriorityModel queuePriority)
        {
            try
            {
                if (queuePriority.Id == Guid.Empty)
                {
                    queuePriority.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    queuePriority.CreatedOn = DateTime.Now;
                    queuePriority.IsNew = true;
                }
                else
                {
                    queuePriority.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    queuePriority.UpdatedOn = DateTime.Now;
                }
                await _queueMessagePriority.AddOrUpdateAsync(_mapper.Map<QueueMessagePriorityVM>(queuePriority));

                return Json(JsonResponseFactory.SuccessResponse("Priorización guardada correctamente."));
            }
            catch (Exception e)
            {
                _logService.Add(LogKey.Error, "QueueMessagePriorityController-CreateOrUpdate: " + e.Message);
                _logService.Generate();
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _queueMessagePriority.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó la priorización correctamente."));
            }
            catch (Exception e)
            {
                _logService.Add(LogKey.Error, "QueueMessagePriorityController-Delete: " + e.Message);
                _logService.Generate();
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(QueueMessagePriorityModel queuePriority)
        {
            string message = string.Empty;

            message = await _queueMessagePriority.Validations(_mapper.Map<QueueMessagePriorityVM>(queuePriority));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        private async Task<List<SelectListItem>> EntityTypeList()
        {
            List<ValueCatalogVM> entityTypes = await _queueMessagePriority.GetEntityTypeList();
            var entityTypesList = Common.LoadList(entityTypes, "Name", "Code");
            if (entityTypesList == null)
            {
                return new List<SelectListItem>();
            }
            return entityTypesList;
        }

        private async Task<List<SelectListItem>> EntityList(int entityType)
        {
            List<QueueMessagePriorityVM> entities = await _queueMessagePriority.GetEntitiesList(entityType);
            var entityList = Common.LoadList(entities, "EntityName", "EntityCode");
            if (entityList == null)
            {
                return new List<SelectListItem>();
            }
            return entityList;
        }

        private List<SelectListItem> PeriodicityList()
        {
            List<ValueCatalogVM> priorities = _queueMessagePriority.PriorityList();
            var priorityList = Common.LoadList(priorities, "Description", "Name");
            if (priorityList == null)
            {
                return new List<SelectListItem>();
            }
            return priorityList;
        }

        [HttpGet]
        public async Task<ActionResult> GetEntityList(int entityType)
        {
            List<QueueMessagePriorityVM> entities = await _queueMessagePriority.GetEntitiesList(entityType);
            var entityList = Common.LoadList(entities, "EntityName", "EntityCode");

            if (entityList == null)
            {
                return Json(new List<QueueMessagePriorityVM>());
            }

            return Json(entityList);
        }

        [HttpPost]
        public async Task<ActionResult> ExportProcessingOrders(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var processingOrders = (await _queueMessagePriority.GetAllAsync()).ToList();

            var newProcessingOrders = (from processingOrder in processingOrders select new { TipoCooperativa = processingOrder.EntityTypeName, Cooperativa = processingOrder.EntityName, Prioridad = processingOrder.Priority, Activo = processingOrder.IsEnabled ? "SI" : "NO" }).OrderBy(t => t.TipoCooperativa).ToList<object>();
            sheetsData.Add(newProcessingOrders);
            sheetsName.Add("Priorización Cargue");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}