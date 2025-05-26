using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.DataAccess.Core;
using Core.Domain.CatalogService;
using Core.Domain.LogService;
using Core.Domain.OfficeService;
using Core.Domain.ReportParameterDefinitionService;
using Core.Domain.ReportParameterService;
using Core.Domain.ReportService;
using Core.Domain.Utils;
using Core.Domain.ViewModel;
using Core.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore, PrivilegesEnum.ViewReports)]
    public class RunReportController : Controller
    {
        #region Private fields
        private readonly IReportService _reportService;
        private readonly IReportParameterService _reporParameterService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportParameterDefinitionService _reportParameterDefinitionService;
        private readonly IOfficeService _officeService;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogService _logService;
        #endregion

        #region Constructor
        public RunReportController(IReportService reportService, IReportParameterService reporParameterService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService, IUnitOfWork unitOfWork, IReportParameterDefinitionService reportParameterDefinitionService, IOfficeService officeService, IHttpContextAccessor accessor, ILogService logService)
        {
            _reportService = reportService;
            _reporParameterService = reporParameterService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
            _unitOfWork = unitOfWork;
            _reportParameterDefinitionService = reportParameterDefinitionService;
            _officeService = officeService;
            _accessor = accessor;
            _logService = logService;
        }
        #endregion
        // GET: RunReport
        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "RunReport/";
            var user = _mapper.Map<UserModelComplete>(_userBasicModel.GetCurrentUser());
            var model = _mapper.Map<List<ReportVM>, List<ReportModel>>((await _reportService.GetAllByRolAsync(user.Roles)).ToList());
            model.ForEach(e => e.RoleNames = (e.ReportPermissionList.Count > 0) ?
                                string.Join(", ", e.ReportPermissionList.Select(x => x.RoleCode)) : "");
            ViewBag.Reports = JsonConvert.SerializeObject((from a in model
                                                           select new
                                                           {
                                                               a.Id,
                                                               a.Name,
                                                               a.Description,
                                                               a.PreView
                                                           }).ToList());
            ViewBag.ReportsList = new SelectList(model.Select(s => new { Value = s.Id, Text = s.Name }).ToList(), "Value", "Text");
            ViewBag.TotalRows = 0;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetReportParameters(Guid id)
        {
            var entityCodeParameter = Constants.EntityCodeParameter;
            var user = _mapper.Map<UserBasicModel>(_userBasicModel.GetCurrentUser());

            List<ReportParameterVM> parametersList = (await _reporParameterService.GetByReportId(id)).ToList();

            var data = from a in parametersList
                       select new
                       {
                           a.ReportParameterDefinition.Id,
                           a.ReportParameterDefinition.Name,
                           a.ReportParameterDefinition.DataType,
                           a.ReportParameterDefinition.ReplaceKey,
                           a.ReportParameterDefinition.ListType,
                           a.ReportParameterDefinition.IsRequired,
                           EntityCode = user.Organization,
                           EntityCodeParameter = entityCodeParameter
                       };

            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> GetListOptions(Guid id)
        {
            var parameter = await _reporParameterService.GetById(id);
            var result = await _reporParameterService.GetResultListParameters(parameter.SQLSentence);
            return Json(result);
        }

        [HttpGet]
        public ActionResult IndexGrid()
        {
            string query = _accessor.HttpContext.Session.GetString("Query");

            try
            {
                if (query.Equals(string.Empty))
                {
                    return PartialView("_IndexGrid", new DataTable());
                }

                DataTable model = _unitOfWork.ExecuteQuery(query).Result;

                return PartialView("_IndexGrid", model);
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Trace: {ex.Trace()}, Error Message: {ex.Message()}, Query: {query}");
                return PartialView("_IndexGrid", GetErrorDataTable("Se presentó una incidencia ejecutando la consulta, contacte al administrador e indíquele el nombre del reporte que está ejecutando junto con sus parámetros."));
            }
            finally
            {
                _logService.Generate();
            }
        }

        [HttpPost]
        public async Task<ActionResult> SetQuery([FromBody] RunReportModel parameters)
        {
            try
            {
                var reporte = await _reportService.GetByIdAsync(parameters.ReportId);
                var query = reporte.SQLSentence;
                foreach (var parameter in parameters.Parameters)
                {
                    var match = "{" + parameter.Id + "}";
                    query = query.Replace(match, "'" + parameter.Value + "'");
                }

                List<string> reservedWords = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.FormuladorPalabrasReservadas))
              .ValueCatalogs.Select(x => x.Name.ToUpper()).ToList();

                if (Utilities.ValidateSQLSentenceReservedWords(query, reservedWords))
                {
                    return BadRequest("Está intentando realizar una operacion prohibida.</b> <br/> La consulta SQL contiene palabras reservadas no permitidas.");
                }

                _logService.Add(LogKey.Begin, "RunReport SetQuery");
                _logService.Add(LogKey.Request, JsonConvert.SerializeObject(query));

                HttpContext.Session.SetString("Query", query);

                return Ok();
            }
            catch (Exception e)
            {
                var err = e.Message();
                return BadRequest("Se presentó una incidencia ejecutando la consulta, inténtelo de nuevo más tarde.");
            }
            finally
            {
                _logService.Add(LogKey.End, "RunReport SetQuery");
                _logService.Generate();
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExportReport(string reportName)
        {
            string query = _accessor.HttpContext.Session.GetString("Query");
            try
            {
                if (query.Equals(string.Empty))
                {
                    return BadRequest("No se pudo ejecutar el reporte");
                }
                List<string> reservedWords = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.FormuladorPalabrasReservadas))
              .ValueCatalogs.Select(x => x.Name.ToUpper()).ToList();

                if (Utilities.ValidateSQLSentenceReservedWords(query, reservedWords))
                {
                    return BadRequest("Está intentando realizar una operacion prohibida.</b> <br/> La consulta SQL contiene palabras reservadas no permitidas.");
                }

                _logService.Add(LogKey.Begin, "RunReport ExportReport");
                _logService.Add(LogKey.Request, JsonConvert.SerializeObject(reportName));
                _logService.Add(LogKey.Request, JsonConvert.SerializeObject(query));

                var model = await _unitOfWork.ExecuteQuery(query);

                reportName = string.Concat(reportName, "_", DateTime.Now.ToString("dd_MM_yyyy_HH_mm"));

                return File(_officeService.GenerateSpreadsheetDocument(model), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportName}.xlsx");
            }
            catch (Exception e)
            {
                var err = e.Message();
                _logService.Add(LogKey.Error, $"Trace: {e.Trace()}, Error Message: {err}, Query: {query}");
                reportName = string.Concat(reportName, "_error");
                return File(_officeService.GenerateSpreadsheetDocument(GetErrorDataTable("Se presentó una incidencia ejecutando la consulta, contacte al administrador e indíquele el nombre del reporte que está ejecutando junto con sus parámetros.")), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportName}.xlsx");
            }
            finally
            {
                _logService.Add(LogKey.End, "RunReport ExportReport");
                _logService.Generate();
            }
        }

        private DataTable GetErrorDataTable(string error)
        {
            DataTable errorTable = new DataTable();

            DataColumn errorColumn = new DataColumn("Error", typeof(string));
            errorTable.Columns.Add(errorColumn);

            DataRow errorRow = errorTable.NewRow();
            errorRow["Error"] = error;
            errorTable.Rows.Add(errorRow);
            errorTable.AcceptChanges();

            return errorTable;
        }
    }
}
