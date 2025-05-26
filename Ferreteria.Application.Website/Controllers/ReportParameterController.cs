using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.DataAccess.Model.Formulator;
using Core.Domain.OfficeService;
using Core.Domain.ReportParameterDefinitionService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class ReportParameterController : Controller
    {
        #region Private fields
        private readonly IReportParameterDefinitionService _reportParameterDefinitionService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IOfficeService _officeService;
        #endregion

        #region Constructor
        public ReportParameterController(IReportParameterDefinitionService ReportParameterDefinitionService, IMapper mapper, IUserBasicModel userBasicModel,
            IOfficeService officeService)
        {
            _reportParameterDefinitionService = ReportParameterDefinitionService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
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
            var model = _mapper.Map<List<ReportParameterDefinitionVM>, List<ReportParameterDefinitionModel>>((await _reportParameterDefinitionService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear Parámetro Reporte";
            ViewBag.ListDataTypes = GetDataTypes().ToListItemsGeneric();
            ViewBag.ListTypes = GetListTypes().ToListItemsGeneric();
            return PartialView("_CreateOrEdit", new ReportParameterDefinitionModel { IsNew = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar Parámetro Reporte";
            var reportParameterDefinition = await _reportParameterDefinitionService.GetByIdAsync(new Guid(id));
            ViewBag.ListDataTypes = GetDataTypes().ToListItemsGeneric();
            ViewBag.ListTypes = GetListTypes().ToListItemsGeneric();

            ReportParameterDefinitionModel model = _mapper.Map<ReportParameterDefinitionModel>(reportParameterDefinition);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(ReportParameterDefinitionModel reportParameterDefinition)
        {
            try
            {
                if (reportParameterDefinition.DataType != "LISTA")
                {
                    reportParameterDefinition.ListType = null;
                    reportParameterDefinition.SQLSentence = null;
                }


                if (reportParameterDefinition.Id == Guid.Empty)
                {
                    reportParameterDefinition.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    reportParameterDefinition.CreatedOn = DateTime.Now;
                    reportParameterDefinition.IsNew = true;
                    reportParameterDefinition.ReplaceKey = "{" + reportParameterDefinition.ReplaceKey + "}";
                }
                else
                {
                    reportParameterDefinition.ReplaceKey = "{" + reportParameterDefinition.ReplaceKey.Replace("{", "").Replace("}", "") + "}";
                    reportParameterDefinition.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    reportParameterDefinition.UpdatedOn = DateTime.Now;
                }
                await _reportParameterDefinitionService.AddOrUpdateAsync(_mapper.Map<ReportParameterDefinitionVM>(reportParameterDefinition));

                return Json(JsonResponseFactory.SuccessResponse("Parámetro guardado correctamente."));
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
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _reportParameterDefinitionService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el parámetro correctamente."));
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
        public async Task<JsonResult> Validations(ReportParameterDefinitionModel reportParameterDefinition)
        {
            string message = string.Empty;
            reportParameterDefinition.ReplaceKey = "{" + reportParameterDefinition.ReplaceKey.Replace("{", "").Replace("}", "") + "}";
            message = await _reportParameterDefinitionService.Validations(_mapper.Map<ReportParameterDefinitionVM>(reportParameterDefinition));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });


        }

        [HttpPost]
        public async Task<ActionResult> ExportData(string fileName)
        {
            List<List<object>> sheetsData = new();
            List<string> sheetsName = new();

            var records = (await _reportParameterDefinitionService.GetAllAsync()).ToList();
            var lstData = records.Select(x => new
            {
                Nombre = x.Name,
                Llave = x.ReplaceKey,
                TipoDato = x.DataType,
                TipoLista = x.ListType == Domain.Utils.ListType.TypeSimple ? Domain.Utils.ListType.Simple : x.ListType == Domain.Utils.ListType.TypeMultiple ? Domain.Utils.ListType.Multiple : "",
                Obligatorio = (x.IsRequired ? "SI" : "NO"),
                SentenciaSQL = x.SQLSentence
            }).ToList<object>();
            sheetsData.Add(lstData);
            sheetsName.Add("Parámetros Reportes");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
        #endregion

        #region Prievate Methods
        private List<SimpleItem> GetDataTypes()
        {
            List<SimpleItem> DataTypeList = new List<SimpleItem>
            {
                new SimpleItem{ Value = "VARCHAR2", Text= "VARCHAR2", Type="VARCHAR2"},
                new SimpleItem{ Value = "DATE", Text= "DATE", Type="DATE"},
                new SimpleItem{ Value = "NUMBER", Text= "NUMBER", Type="NUMBER"},
                new SimpleItem{ Value = "LISTA", Text= "LISTA", Type="LISTA"}
            };
            return DataTypeList;
        }

        private List<SimpleItem> GetListTypes()
        {
            List<SimpleItem> DataTypeList = new List<SimpleItem>
            {
                new SimpleItem{ Value = Domain.Utils.ListType.TypeSimple.ToString(), Text = Domain.Utils.ListType.Simple},
                new SimpleItem{ Value = Domain.Utils.ListType.TypeMultiple.ToString(), Text = Domain.Utils.ListType.Multiple}
            };

            return DataTypeList;
        }
        #endregion
    }
}