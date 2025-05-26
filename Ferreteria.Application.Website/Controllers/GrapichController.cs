using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.GraphicGroupService;
using Core.Domain.GraphicIndicatorService;
using Core.Domain.GraphicSubGroupService;
using Core.Domain.IndicatorService;
using Core.Domain.OfficeService;
using Core.Domain.QueueMessagePriorityService;
using Core.Domain.Utils;
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
    public class GraphicController : Controller
    {
        #region Private fields
        private readonly IConfigurator _configurationService;
        private readonly IGraphicService _graphicService;
        private readonly IGraphicGroupService _graphicGroupService;
        private readonly IGraphicSubGroupService _graphicSubGroupService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IIndicatorService _indicatorService;
        private readonly ICatalogService _catalogService;
        private readonly IGraphicIndicatorService _graphicIndicatorService;
        private readonly IOfficeService _officeService;

        #endregion

        #region Constructor
        public GraphicController(IGraphicService graphicService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService,
                                IGraphicGroupService graphicGroupService, IGraphicSubGroupService graphicSubGroupService, IIndicatorService indicatorService,
                                IGraphicIndicatorService graphicIndicatorService, IOfficeService officeService)
        {
            _graphicService = graphicService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
            _graphicGroupService = graphicGroupService;
            _graphicSubGroupService = graphicSubGroupService;
            _indicatorService = indicatorService;
            _graphicIndicatorService = graphicIndicatorService;
            _officeService = officeService;
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<GraphicVM>, List<GraphicModel>>((await _graphicService.GetAllAsync()).ToList());
            model.ForEach(e => e.RoleNames = (e.GraphicPermissionList.Count > 0) ?
                               string.Join(", ", e.GraphicPermissionList.Select(x => x.RoleCode)) : "");
            model.ForEach(e => e.IndicatorsNames = (e.ListGraphicIndicator.Count > 0) ?
                               string.Join(", ", e.ListGraphicIndicator.Select(x => x.Indicator.Name)) : "");
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<ViewResult> Create()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TitlePage = "Crear Gráfica";
            ViewBag.ListRolePermissions = GetListRolePermissions().Result;
            ViewBag.ListGraphicGroup = GetListGraphicGroup().Result;
            ViewBag.ListTypeGraphic = _graphicService.GetTypeGraphicList();
            ViewBag.ListPositionLegend = _graphicService.GetPositionLegendList();
            ViewBag.IndicatorsList = GetInidicatorList().Result;
            ViewBag.TypeFieldList = _graphicService.GetTypeFieldList();
            ViewBag.TypeEjeList = _graphicService.GetTypeEjeList();
            ViewBag.SignIndicatorList = _graphicService.GetSignIndicatorList();
            ViewBag.IndicatorAggregateList = GetListIndicatorAggregate().Result;
            ViewBag.ListGraphicSubGroup = new List<SelectListItem>();
            ViewBag.ListCooperativeType = (await _catalogService.GetByCodeAsync("TiposFormato")).ValueCatalogs.ToLoadGenericList("Name", "Code");

            return View("CreateOrEdit", new GraphicModel { IsNew = true });
        }

        [HttpPost]
        public async Task<ViewResult> Edit(string id)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TitlePage = "Editar Gráfica";
            var graphic = await _graphicService.GetByIdAsync(new Guid(id));
            ViewBag.ListRolePermissions = GetListRolePermissions().Result;
            ViewBag.ListGraphicGroup = GetListGraphicGroup().Result;
            ViewBag.ListTypeGraphic = _graphicService.GetTypeGraphicList();
            ViewBag.IndicatorsList = GetInidicatorList().Result;
            ViewBag.TypeFieldList = _graphicService.GetTypeFieldList();
            ViewBag.TypeEjeList = _graphicService.GetTypeEjeList();
            ViewBag.SignIndicatorList = _graphicService.GetSignIndicatorList();
            ViewBag.ListPositionLegend = _graphicService.GetPositionLegendList();
            ViewBag.ListGraphicSubGroup = GetListGraphicSubGroup(graphic.GraphicGroupId).Result;
            ViewBag.IndicatorAggregateList = GetListIndicatorAggregate().Result;
            GraphicModel model = _mapper.Map<GraphicModel>(graphic);
            ViewBag.ListGraphicIndicator = model.ListGraphicIndicator;
            ViewBag.ListCooperativeType = (await _catalogService.GetByCodeAsync("TiposFormato")).ValueCatalogs.ToLoadGenericList("Name", "Code");

            return View("CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> GraphicDetails(string id)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            var graphicModel = await _graphicService.GetByIdAsync(new Guid(id));
            ViewBag.ListRolePermissions = GetListRolePermissions().Result;
            GraphicModel model = _mapper.Map<GraphicModel>(graphicModel);

            ViewBag.TitlePage = $"Detalles Gráfica - {model.Title}";
            return PartialView("_GraphicDetails", model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteIndicator(Guid id)
        {
            try
            {
                await _graphicIndicatorService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el registro correctamente."));
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



        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                if (id != Guid.Empty)
                {
                    var graphicModel = await _graphicService.GetByIdAsync(id);

                    graphicModel.IsEnabled = false;
                    graphicModel.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    graphicModel.UpdatedOn = DateTime.Now;
                    await _graphicService.AddOrUpdateAsync(graphicModel);
                }

                return Json(JsonResponseFactory.SuccessResponse("Se anactivó el registro correctamente."));
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("||") >= 0)
                {
                    return Json(JsonResponseFactory.ErrorResponse(ex.Message.Replace("||", "")));
                }
                return Json(JsonResponseFactory.ErrorResponse("No fue posible anactivar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(GraphicModel graphicModel)
        {
            try
            {
                if (graphicModel.Id == Guid.Empty)
                {
                    graphicModel.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    graphicModel.CreatedOn = DateTime.Now;
                    graphicModel.IsNew = true;
                    graphicModel.IsEnabled = true;
                }
                else
                {
                    graphicModel.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    graphicModel.UpdatedOn = DateTime.Now;
                }
                await _graphicService.AddOrUpdateAsync(_mapper.Map<GraphicVM>(graphicModel));

                return Json(JsonResponseFactory.SuccessResponse("Gráfica guardada correctamente."));
            }
            catch (Exception e)
            {
                var err = e.Message;
                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El nombre ingresado ya existe."));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(GraphicModel graphicModel)
        {
            string message = string.Empty;

            message = await _graphicService.Validations(_mapper.Map<GraphicVM>(graphicModel));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpGet]
        public async Task<JsonResult> GetSubGroups(Guid groupId)
        {
            List<GraphicSubGroupVM> lst = (await _graphicSubGroupService.GetAllByGraphicGroupIdAsync(groupId)).ToList();
            var lstObject = lst.Select(x => new { Name = x.Name, Id = x.Id }).ToList();

            if (lstObject == null)
            {
                return Json(new());
            }

            return Json(lstObject);
        }

        [HttpPost]
        public async Task<ActionResult> ExportData(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var graphics = (await _graphicService.GetAllAsync()).ToList();
            var graphicIndicators = (await _graphicIndicatorService.GetAllAsync()).ToList();
            var cooperativeType = (await _catalogService.GetByCodeAsync("TiposFormato")).ValueCatalogs;

            var newGraphics = graphics.Select(x => new
            {
                TipoCooperativa = x.EntityTypeId != null ? cooperativeType.Where(c => c.Code == x.EntityTypeId).FirstOrDefault().Name : "",
                Grupo = x.GraphicGroup.Name,
                SubGrupo = x.GraphicSubGroup != null ? x.GraphicSubGroup.Name : "",
                Nombre = x.Title,
                SubTitulo = x.SubTitle,
                TipoGrafica = x.TypeGraphic,
                Roles = (x.GraphicPermissionList.Count > 0 ?
                               string.Join(", ", x.GraphicPermissionList.Select(x => x.RoleCode)) : ""),
                Orden = x.Order,
                Activo = x.IsEnabled ? "SI" : "NO",
                DescargarInformacion = x.IsDownloadable ? "SI" : "NO",
                UsaCuentasBalances = x.UseAccount ? "SI" : "NO",
                TituloEnEjeY = x.AxisTitleY,
                TituloEnEjeY1 = x.AxisTitleY1,
                TituloEnEjeX = x.AxisTitleX,
                PosicíonLenyendas = x.PositionLegends == "bottom" ? "Abajo" : x.PositionLegends == "top" ? "Arriba" : x.PositionLegends == "left" ? "Izquierda" : x.PositionLegends == "right" ? "Derecha" : "",
                SentenciaSQL = x.SQLSentence,
                SentenciaSQLComplementaria = x.SQLSentenceSupplementary,
            }).ToList<object>();

            var newGraphicIndicator = graphicIndicators.Select(x => new
            {
                Grafica = x.Graphic.Title,
                Orden = x.Order,
                Indicador = x.Indicator.Name,
                Eje = x.Axis,
                TipoCampo = x.TypeField,
                Leyenda = x.DisplayText,
                Signo = x.Sign,
                Agregado = x.IndicatorAggregateId != null ? x.IndicatorAggregate.Name : "",
                x.Color
            }).OrderBy(x => x.Grafica).ToList<object>();

            sheetsData.Add(newGraphics);
            sheetsName.Add("Gráficas");
            sheetsData.Add(newGraphicIndicator);
            sheetsName.Add("Indicadores de las gráficas");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }

        #endregion

        #region Prievate Methods

        private async Task<List<SelectListItem>> GetInidicatorList()
        {
            List<IndicatorVM> listIndicators = (await _indicatorService.GetAllAsync()).Where(x => x.IsEnabled).ToList();
            listIndicators.ForEach(x => x.Name = string.Concat(x.ValueCatalogGroupIndicator.Name, " - ", x.Name));
            var indicatorsList = Common.LoadList(listIndicators, "Name", "Id");
            if (indicatorsList == null)
            {
                return new List<SelectListItem>();
            }
            return indicatorsList;
        }

        private async Task<List<SelectListItem>> GetListGraphicSubGroup(Guid groupId)
        {
            List<GraphicSubGroupVM> lst = (await _graphicSubGroupService.GetAllByGraphicGroupIdAsync(groupId)).ToList();
            var list = Common.LoadList(lst, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> GetListRolePermissions()
        {
            List<ValueCatalogVM> permissions = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.ReportPermissions)).ValueCatalogs;
            var list = Common.LoadList(permissions, "Name", "Code");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> GetListIndicatorAggregate()
        {
            List<ValueCatalogVM> listCatalog = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.CodigosAgregados)).ValueCatalogs;
            var list = Common.LoadList(listCatalog, "Description", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> GetListGraphicGroup()
        {
            List<GraphicGroupVM> groups = (await _graphicGroupService.GetAllAsync()).ToList();
            var list = Common.LoadList(groups, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        #endregion
    }
}
