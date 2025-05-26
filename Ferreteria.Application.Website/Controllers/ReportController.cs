using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.FormulatorService;
using Core.Domain.OfficeService;
using Core.Domain.ReportParameterDefinitionService;
using Core.Domain.ReportService;
using Core.Domain.Utils;
using Core.Domain.ViewModel;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class ReportController : Controller
    {
        #region Private fields
        private readonly IConfigurator _configurationService;
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;
        private readonly IFormulatorService _formulatorService;
        private readonly IReportParameterDefinitionService _reportParameterDefinitionService;
        private readonly IOfficeService _officeService;
        #endregion

        #region Constructor
        public ReportController(IReportService reportService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService,
                                IReportParameterDefinitionService reportParameterDefinitionService, IFormulatorService formulatorService,
                                IConfigurator configurationService, IOfficeService officeService)
        {
            _reportService = reportService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
            _formulatorService = formulatorService;
            _configurationService = configurationService;
            _reportParameterDefinitionService = reportParameterDefinitionService;
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
            var model = _mapper.Map<List<ReportVM>, List<ReportModel>>((await _reportService.GetAllAsync()).ToList());
            model.ForEach(e => e.RoleNames = (e.ReportPermissionList.Count > 0) ?
                                string.Join(", ", e.ReportPermissionList.Select(x => x.RoleCode)) : "");
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public ViewResult Create()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TitlePage = "Crear Reporte";
            ViewBag.ListReportParameters = GetListReportParameters().Result;
            ViewBag.ListRolePermissions = GetListRolePermissions().Result;
            ViewBag.ListTable = GetListTables().Result;
            ViewBag.ListColumns = new List<SelectListItem>();
            return View("CreateOrEdit", new ReportModel { IsNew = true });
        }

        [HttpGet]
        public JsonResult GetListParameters()
        {
            try
            {
                var parameters = _reportParameterDefinitionService.Get().Select(x => new { x.Id, x.Name, x.ReplaceKey });
                var comparisionOperators = _formulatorService.ComparisonOperatorsReport().Result;
                var arithmeticOperators = _formulatorService.ArithmeticOperators().Result;
                var logicOperators = _formulatorService.LogicOperators().Result;
                return Json(new
                {
                    parameters,
                    comparisionOperators,
                    arithmeticOperators,
                    logicOperators,
                });
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<ViewResult> Edit(string id)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TitlePage = "Editar Reporte";
            var report = await _reportService.GetByIdAsync(new Guid(id));
            ViewBag.ListReportParameters = GetListReportParameters().Result;
            ViewBag.ListRolePermissions = GetListRolePermissions().Result;
            ViewBag.ListColumns = GetColumnsList(report.ReportColumnList.FirstOrDefault()).Result;
            ViewBag.ListTable = GetListTables().Result;
            ReportModel model = _mapper.Map<ReportModel>(report);
            return View("CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> ReportDetails(string id)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            var reportModel = await _reportService.GetByIdAsync(new Guid(id));
            ViewBag.ListReportParameters = GetListReportParameters().Result;
            ViewBag.ListRolePermissions = GetListRolePermissions().Result;

            ReportModel model = _mapper.Map<ReportModel>(reportModel);

            ViewBag.TitlePage = $"Detalles Reporte - {model.Name}";
            return PartialView("_ReportDetails", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetColumns(string tableName)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);

            List<string> lstColumns = await _reportService.GetColumns(tableName);
            var lstObject = lstColumns.Select(x => new { Name = x, Id = x }).ToList();

            if (lstObject == null)
            {
                return Json(new());
            }

            return Json(lstObject);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(ReportModel reportModel)
        {
            try
            {
                if (reportModel.Id == Guid.Empty)
                {
                    reportModel.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    reportModel.CreatedOn = DateTime.Now;
                    reportModel.IsNew = true;
                    reportModel.IsEnabled = true;
                }
                else
                {
                    reportModel.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    reportModel.UpdatedOn = DateTime.Now;
                }
                await _reportService.AddOrUpdateAsync(_mapper.Map<ReportVM>(reportModel));

                return Json(JsonResponseFactory.SuccessResponse("Reporte guardado correctamente."));
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

        [HttpGet]
        public async Task<JsonResult> GetListTablesWithType()
        {
            List<object> lst = new();
            List<ValueCatalogVM> tables = new();
            List<ValueCatalogVM> nameTablesCore = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.TablasReporteSIDCORE)).ValueCatalogs;
            List<ValueCatalogVM> nameReportSchemaexternal = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.TablasReporteEsquemaExterno)).ValueCatalogs;

            tables.AddRange(nameTablesCore);
            tables.AddRange(nameReportSchemaexternal);


            lst.AddRange(nameTablesCore.Select(x => new { x.Code, x.Name }));
            lst.AddRange(nameReportSchemaexternal.Select(x => new { x.Code, x.Name }));

            return Json(lst);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _reportService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el reporte correctamente."));
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
        public async Task<JsonResult> Validations(ReportModel reportModel)
        {
            string message = string.Empty;

            message = await _reportService.Validations(_mapper.Map<ReportVM>(reportModel));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> ExportData(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var reports = (await _reportService.GetAllAsync()).ToList();

            var newReports = reports.Select(x => new
            {
                Nombre = x.Name,
                Descripcion = x.Description,
                Activo = (x.IsEnabled ? "SI" : "NO"),
                VistaPrevia = (x.PreView ? "SI" : "NO"),
                SoloSQL = (x.IsOnlySQL ? "SI" : "NO"),
                TiposUsuarios = (x.ReportPermissionList.Count > 0 ? string.Join(", ", x.ReportPermissionList.Select(x => x.RoleCode)) : ""),
                SentenciaSQL = x.SQLSentence
            }).ToList<object>();

            sheetsData.Add(newReports);
            sheetsName.Add("Reportes");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }

        #endregion

        #region Prievate Methods
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

        private async Task<List<SelectListItem>> GetListTables()
        {
            List<ValueCatalogVM> tables = new();
            List<ValueCatalogVM> nameTablesCore = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.TablasReporteSIDCORE)).ValueCatalogs;
            List<ValueCatalogVM> nameTablesExternal = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.TablasReporteEsquemaExterno)).ValueCatalogs;

            tables.AddRange(nameTablesCore);
            tables.AddRange(nameTablesExternal);

            var list = Common.LoadList(tables, "Name", "Code");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        public async Task<List<SelectListItem>> GetColumnsList(ReportColumnVM reportColumn)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            if (reportColumn == null)
            {
                return new List<SelectListItem>();
            }

            List<string> lstColumns = await _reportService.GetColumns(reportColumn.Table.Code);
            var lstObject = lstColumns.Select(x => new { Name = x, Id = x }).ToList();

            var lstCol = Common.LoadList(lstObject, "Name", "Id");

            if (lstCol == null)
            {
                return new List<SelectListItem>();
            }

            return lstCol;
        }

        private async Task<List<SelectListItem>> GetListReportParameters()
        {
            List<ReportParameterDefinitionVM> permissions = (await _reportParameterDefinitionService.GetAllAsync()).ToList();
            var list = Common.LoadList(permissions, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        #endregion
    }
}