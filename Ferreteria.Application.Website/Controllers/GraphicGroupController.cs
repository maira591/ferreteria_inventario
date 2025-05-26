using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.GraphicGroupService;
using Core.Domain.GraphicSubGroupService;
using Core.Domain.LogService;
using Core.Domain.OfficeService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class GraphicGroupController : Controller
    {
        #region Private fields

        private readonly IGraphicGroupService _graphicGroupService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ILogService _logService;
        private readonly IOfficeService _officeService;
        private readonly IGraphicSubGroupService _graphicSubGroupService;

        #endregion

        #region Constructor
        public GraphicGroupController(IGraphicGroupService graphicGroupService, IMapper mapper, IUserBasicModel userBasicModel, ILogService logService,
            IOfficeService officeService, IGraphicSubGroupService graphicSubGroupService)
        {
            _graphicGroupService = graphicGroupService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _logService = logService;
            _officeService = officeService;
            _graphicSubGroupService = graphicSubGroupService;
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
            var model = _mapper.Map<List<GraphicGroupVM>, List<GraphicGroupModel>>((await _graphicGroupService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear grupo gráfico";
            return PartialView("_CreateOrEdit");
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar grupo gráfico";
            var graphicGroup = await _graphicGroupService.GetByIdAsync(new Guid(id));

            return PartialView("_CreateOrEdit", _mapper.Map<GraphicGroupModel>(graphicGroup));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(GraphicGroupModel graphicGroup)
        {
            try
            {
                if (graphicGroup.Id == Guid.Empty)
                {
                    graphicGroup.CreatedBy = _userBasicModel.GetCurrentUser().UserName;
                    graphicGroup.CreatedOn = DateTime.Now;
                    graphicGroup.IsNew = true;
                }
                else
                {
                    graphicGroup.UpdatedBy = _userBasicModel.GetCurrentUser().UserName;
                    graphicGroup.UpdatedOn = DateTime.Now;
                }

                await _graphicGroupService.AddOrUpdateAsync(_mapper.Map<GraphicGroupVM>(graphicGroup));

                return Json(JsonResponseFactory.SuccessResponse("Información guardada correctamente."));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Trace: {ex.StackTrace}, Error Message: {ex.Message}, Entity: {JsonConvert.SerializeObject(graphicGroup)}");
                _logger.Error(string.Concat("Error en GraphicGroupController - CreateOrUpdate: ", ex.Message));
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
            finally
            {
                _logService.Generate();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _graphicGroupService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el registro correctamente."));
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("||") >= 0)
                {
                    return Json(JsonResponseFactory.ErrorResponse(ex.Message.Replace("||", "")));
                }
                else
                {
                    _logger.Error(string.Concat("Error en GraphicGroupController - Delete: ", ex.Message));
                }
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(GraphicGroupModel graphicGroup)
        {
            string message = string.Empty;

            message = await _graphicGroupService.Validations(_mapper.Map<GraphicGroupVM>(graphicGroup));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> ExportData(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var graphicGroups = (await _graphicGroupService.GetAllAsync()).ToList();
            var graphicSubGroups = (await _graphicSubGroupService.GetAllAsync()).ToList();

            var dataGraphicGroup = graphicGroups.Select(x => new { Nombre = x.Name, Descripcion = x.Description }).ToList<object>();
            var dataSubGraphicGroup = graphicSubGroups.Select(x => new { GrupoGrafico = x.GraphicGroup.Name, Nombre = x.Name, Descripcion = x.Description }).OrderBy(x => x.GrupoGrafico).ToList<object>();

            sheetsData.Add(dataGraphicGroup);
            sheetsName.Add("Grupos Gráficos");
            sheetsData.Add(dataSubGraphicGroup);
            sheetsName.Add("Sub Grupos Gráficos");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
        #endregion
    }
}