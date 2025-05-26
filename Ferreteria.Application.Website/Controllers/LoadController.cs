using Core.Application.Website.Helpers;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.CooperativeService;
using Core.Domain.FormatValidationEventsService;
using Core.Domain.FtpService;
using Core.Domain.GeneralNotificationService;
using Core.Domain.LoadInformationService;
using Core.Domain.LoadTypeService;
using Core.Domain.LogService;
using Core.Domain.MaximunDateService;
using Core.Domain.MethodParameter;
using Core.Domain.NotifyService;
using Core.Domain.OfficeService;
using Core.Domain.PeriodicityService;
using Core.Domain.ProcessFlowStateService;
using Core.Domain.RetransmissionService;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Core.Domain.ViewModel.Notification;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore, PrivilegesEnum.Load)]
    public class LoadController : Controller
    {
        #region Private fields
        private readonly IUserBasicModel _userBasicModel;
        private readonly UserBasicModel _currentUser;
        private IFtpService _ftpService;
        private IProcessFlowStateService _processFlowStateService;
        private ICatalogService _catalogService;
        private IValueCatalogService _valueCatalogService;
        private IGeneralNotificationService _generalNotificationService;
        private ILogService _logService;
        private INotifyService _notifyService;
        private IHttpContextAccessor _httpContextAccessor;
        private IFormatValidationEventsService _formatValidationEventsService;
        private ILoadTypeService _loadTypeService;
        private IPeriodicityService _periodicityService;
        private IMaximunDateService _maximunDateService;
        private readonly ILoadInformationService _loadInformationService;
        private readonly IRetransmissionService _retransmissionService;

        private readonly string dataError = "N/A";
        private IOfficeService _officeService;

        private enum ErrorType
        {
            Format = 1,
            Data = 2
        }

        private readonly ICooperativeService _cooperativeService;

        #endregion

        #region Constructor
        public LoadController(IFtpService ftpService, IProcessFlowStateService statusLoadingProcessService, ICatalogService catalogService,
                              IValueCatalogService valueCatalogService, ILoadTypeService loadTypeService, IPeriodicityService periodicityService,
                              INotifyService notifyService, IHttpContextAccessor httpContextAccessor, IGeneralNotificationService generalNotificationService,
                              ILogService logService, IUserBasicModel userBasicModel,
                              IFormatValidationEventsService formatValidationEventsService, IHubContext<FormatValidationsHub> hubContext,
                              IOfficeService officeService, IMaximunDateService maximunDateService,
                              ICooperativeService cooperativeService, ILoadInformationService loadInformationService, IRetransmissionService retransmissionService)
        {
            _ftpService = ftpService;
            _processFlowStateService = statusLoadingProcessService;
            _maximunDateService = maximunDateService;
            _catalogService = catalogService;
            _valueCatalogService = valueCatalogService;
            _loadTypeService = loadTypeService;
            _periodicityService = periodicityService;
            _notifyService = notifyService;
            _formatValidationEventsService = formatValidationEventsService;
            _httpContextAccessor = httpContextAccessor;
            _generalNotificationService = generalNotificationService;
            _logService = logService;
            _userBasicModel = userBasicModel;
            _currentUser = _userBasicModel.GetCurrentUser();
            _officeService = officeService;
            _cooperativeService = cooperativeService;
            _loadInformationService = loadInformationService;
            _retransmissionService = retransmissionService;
        }
        #endregion

        public async Task<IActionResult> Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request) + "Load/";
            //var model = new ZipFileVM { Cuts = new List<SelectListItem>().ToListCuts() };
            var model = new ZipFileVM();
            string entityType = string.Empty;
            ViewBag.DisabledCooperativeList = false;
            if (!string.IsNullOrWhiteSpace(_currentUser.Organization))
            {
                var cooperative = await _cooperativeService.GetByCode(int.Parse(_currentUser.Organization));
                model.OrganizationCode = _currentUser.Organization;
                model.OrganizationName = cooperative.Name;
                entityType = cooperative.CooperativeType.ToString();
                ViewBag.DisabledCooperativeList = true;
            }
            else
            {
                model.OrganizationCode = _currentUser.Organization;
                model.OrganizationName = dataError;
            }
            ViewBag.isBtnLoadTryAgainActive = Convert.ToBoolean(_valueCatalogService.GetByCode("isBtnLoadTryAgainActive").Name);
            ViewBag.Interval = _ftpService.GetIntervalTime();

            var cooperativesList = _catalogService.GetByCode("TiposFormato").ValueCatalogs;
            var selectedItem = cooperativesList.Where(x => x.Code == entityType).FirstOrDefault();
            if (selectedItem == null)
            {
                selectedItem = new ValueCatalogVM();
            }

            ViewBag.CooperativeType = new SelectList(cooperativesList.Select(s => new { Value = s.Id, Text = s.Name }).ToList(), "Value", "Text", selectedItem.Id);
            var loadTypeList = await _loadTypeService.GetByCooperativeTypeAsync(selectedItem.Id);
            ViewBag.LoadType = new SelectList(loadTypeList.Select(s => new { Value = s.Id, Text = s.Name }).ToList(), "Value", "Text");

            return View(model);
        }

        [HttpGet]
        public JsonResult GenerateDate(string loadTypeId)
        {
            var loadType = _loadTypeService.GetById(Guid.Parse(loadTypeId));
            var periodicity = _periodicityService.GetByIdAsync(loadType.PeriodicityId);

            var periodicityDays = periodicity.Result.Days;
            var monthsAverage = PeriodicityType.PromedioDiasMeses;
            var months = 0;
            if (periodicityDays >= PeriodicityType.TreintaDias)
            {
                var result = periodicityDays / monthsAverage;
                months = Convert.ToInt32(result);
            }

            //Si los meses maximos para calcular los cortes no estan en valores catalogos, toma el 12 por defecto.
            int maxMonthToCalculateCuts = PeriodicityType.DoceMeses;
            ValueCatalogVM valueMaxMonthCuts = _valueCatalogService.GetByCode(ConstantsValueCatalogs.MaximoMesesCorte);
            if (valueMaxMonthCuts != null)
            {
                maxMonthToCalculateCuts = int.Parse(valueMaxMonthCuts.Name);
            }

            var getCuts = new List<SelectListItem>().ToListCuts(periodicity.Result.Name, months, maxMonthToCalculateCuts);
            var Cuts = new SelectList(getCuts.Select(s => new { Value = s.Value, Text = s.Text }).ToList(), "Value", "Text");
            return new JsonResult(Cuts);
        }

        [HttpGet]
        public JsonResult GetPeriodicityType(string loadTypeId)
        {
            var loadType = _loadTypeService.GetById(Guid.Parse(loadTypeId));
            var periodicity = _periodicityService.GetById(loadType.PeriodicityId);
            return new JsonResult(periodicity.Name);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateFile(ZipFileVM model)
        {
            var loadType = await _loadTypeService.GetByIdAsync(Guid.Parse(model.LoadType));
            return Json(await _ftpService.ValidateFile(model.AttachedFile, model.CooperativeType, model.CutDate, model.OrganizationCode, loadType.PeriodicityId));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(ZipFileVM modelFile)
        {
            try
            {
                #region Se sube el archivo al FTP
                var fechaCorte = Convert.ToDateTime(modelFile.CutDate);

                var folderStructureResult = await _ftpService.CheckFolderStructure(modelFile.OrganizationCode, modelFile.CutDate, true);
                if (!folderStructureResult.IsValid)
                    return new JsonResult
                    (
                         folderStructureResult
                    );

                var result = await _ftpService.UploadFile(true, modelFile.OrganizationCode, modelFile.CutDate, modelFile.AttachedFile);

                if (result.StatusCode != FtpStatusCode.ClosingData)
                {
                    return new JsonResult(
                        new FormatResultVM
                        {
                            IsValid = false,
                            Results = new List<ValidationResultVM>
                            {
                                new ValidationResultVM
                                {
                                    Msg = "No se logró cargar el archivo en el folder en el FTP.",
                                    IsValid = false
                                }
                            }
                        }
                    );
                }
                #endregion

                return new JsonResult(

                    new FormatResultVM
                    {
                        IsValid = true,
                        IsUpload = true,
                        Results = new List<ValidationResultVM>
                        {
                            new ValidationResultVM
                            {
                                Msg = "",
                                IsValid = true
                            }
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                return new JsonResult
                (
                    new FormatResultVM
                    {
                        IsValid = false,
                        IsUpload = false,
                        Results = new List<ValidationResultVM>
                        {
                            new ValidationResultVM
                            {
                                Msg = ex.Message,
                                IsValid = false
                            }
                        }
                    }
                    );
            };
        }


        [HttpPost]
        public async Task<JsonResult> EnQueue(string codigoEntidad, Guid loadType, Guid cooperativeType, string fechaCorte, Guid agrupador)
        {
            #region RabbitMQ Message Queue
            bool successEnQueue = false;
            string estado = string.Empty;

            try
            {
                var tyeCoopertiveCode = _valueCatalogService.GetById(cooperativeType).Code;
                var loadInformationToSave = GetLoadInformationVM(codigoEntidad, fechaCorte, _currentUser.UserName, tyeCoopertiveCode);
                await _loadInformationService.AddOrUpdateAsync(loadInformationToSave);

                var parameterProcessPost = new ParameterProcessPost
                {
                    EntityCode = codigoEntidad,
                    CutoffDate = fechaCorte,
                    LoadType = loadType,
                    TypeCooperative = cooperativeType,
                    UploadedBy = _currentUser.UserName,
                    EmailCooperative = _currentUser.Email,
                    NameCooperative = _currentUser.Name,
                    IsRetransmission = loadInformationToSave.IsRetransmission
                };
                var queueResponse = _processFlowStateService.Enqueue(parameterProcessPost);

                var msg = $"Archivo puesto en cola de procesamiento: <b>{queueResponse.Code} - {queueResponse.Message}</b>.";

                var processFlowState = new ProcessFlowStateVM()
                {
                    CodigoEntidad = int.Parse(codigoEntidad),
                    FechaCorte = Utilities.DateParse(fechaCorte).Value,
                    CreatedBy = _currentUser.UserName,
                    Agrupador = agrupador,
                    Detalles = msg
                };
                var statusCodeOk = ((int)HttpStatusCode.OK).ToString();

                if (queueResponse.Code != statusCodeOk)
                {
                    //forzar fin del proceso
                    estado = "ErrorEncolamiento";
                    var estateValueCatalog = _valueCatalogService.GetByCode(estado);
                    var processValueCatalog = _valueCatalogService.GetByCode("Encolamiento");
                    processFlowState.Estado = estateValueCatalog.Id;
                    processFlowState.Proceso = processValueCatalog.Id;
                    await _processFlowStateService.AddOrUpdateAsync(processFlowState);

                    var estateFinProceso = _valueCatalogService.GetByCode("FinProcesarFormatos");
                    processFlowState.Id = Guid.Empty;
                    processFlowState.Estado = estateFinProceso.Id;
                    processFlowState.Detalles = "Se detuvo el proceso.";
                    await _processFlowStateService.AddOrUpdateAsync(processFlowState);

                    //Actualizar si hay error
                    var loadInformation = await _loadInformationService.GetLoadInformationByState(Utilities.DateParse(fechaCorte).Value, codigoEntidad, ConstantsLoadInformation.EnProceso);
                    if (loadInformation != null)
                    {
                        loadInformation.UpdatedOn = DateTime.Now;
                        loadInformation.UpdatedBy = _currentUser.UserName;
                        loadInformation.State = ConstantsLoadInformation.Rechazado;
                        loadInformation.Observation = "Los datos no superaron las validaciones.";

                        await _loadInformationService.AddOrUpdateAsync(loadInformation);
                    }
                }
                else
                {
                    successEnQueue = true;
                    estado = "Encola";
                    var estateValueCatalog = _valueCatalogService.GetByCode(estado);
                    var processValueCatalog = _valueCatalogService.GetByCode("ProcesamientoZip");
                    processFlowState.Estado = estateValueCatalog.Id;
                    processFlowState.Proceso = processValueCatalog.Id;
                    await _processFlowStateService.AddOrUpdateAsync(processFlowState);
                }
            }
            catch
            {
                estado = "FinProcesoError";
                successEnQueue = false;
            }
            return new JsonResult(new { encola = successEnQueue, Estado = estado });

            #endregion
        }


        [HttpPost]
        public JsonResult ValidateCutDate(string cutDate)
        {
            var dateCutDate = Utilities.DateParse(cutDate);
            var currentCut = DateTime.Now;
            var isValid = new DateTime(currentCut.Year, currentCut.Month, DateTime.DaysInMonth(currentCut.Year, currentCut.Month)) == dateCutDate;
            var ammountCutsToRecalculate = (!isValid) ? $"{dateCutDate:Y} a {currentCut:Y}" : "";
            return new JsonResult(new { isValid, ammountCutsToRecalculate });
        }

        [HttpPost]
        public async Task<JsonResult> ProcessState(string codigoEntidad, string fechaCorte)
        {
            try
            {
                var data = _processFlowStateService.GetAllByLastGrouper(codigoEntidad, Utilities.DateParse(fechaCorte).Value);
                var isFinishProcess = data.Any() &&
                                      data.FirstOrDefault(d => d.EstadoValorCatalogo.Code.Equals("FinProcesamientoZip")) != null;
                var isError = data.FirstOrDefault(d => d.EstadoValorCatalogo.Code.Equals("ErrorProcesamientoZip") ||
                                                                       d.EstadoValorCatalogo.Code.Equals("ErrorEncolamiento")) != null;

                foreach (var t in data)
                {
                    var detalle = await _generalNotificationService.GetMessage(t.Detalles);

                    if (!detalle.Equals(t.Detalles))
                    {
                        t.Detalles =
                            $"{detalle} <h2>Error Técnico</h2>{t.Detalles}";
                    }
                }

                var listResults = data.Select(a =>
                                     new
                                     {
                                         Date = a.CreatedOn.ToString(CultureInfo.CurrentCulture),
                                         a.Detalles,
                                         _valueCatalogService.GetById(a.Estado).Code,
                                         _valueCatalogService.GetById(a.Estado).Description,
                                         _valueCatalogService.GetById(a.Estado).Name
                                     }).ToList();
                var lastResultGN = listResults.FirstOrDefault();
                listResults.Remove(lastResultGN);

                return new JsonResult
                (new
                {
                    lastResult = lastResultGN,
                    uploadResults = listResults,
                    isFinishProcess,
                    isError,
                    lastFlowState = data.FirstOrDefault()?.EstadoValorCatalogo.Code
                }
                );
            }
            catch (Exception e)
            {
                var err = e.Message;
                _logService.Add(LogKey.Error, $"Trace: {e.StackTrace}, Error Message: {err}");
                _logService.Generate();
                throw new Exception(err);
            }
        }

        [HttpPost]
        public async Task AddFlowProccess(int codigoEntidad, string fechaCorte, string codigoEstado, string codigoProceso, Guid agrupador)
        {
            var estateValueCatalog = _valueCatalogService.GetByCode(codigoEstado)?.Id;
            var processValueCatalog = _valueCatalogService.GetByCode(codigoProceso)?.Id;

            var userId = _currentUser.UserName;
            var flujoProcesoCarga = new ProcessFlowStateVM
            {
                Agrupador = agrupador,
                CodigoEntidad = codigoEntidad,
                Estado = estateValueCatalog.Value,
                FechaCorte = Utilities.DateParse(fechaCorte).Value,
                Proceso = processValueCatalog.Value,
                Detalles = $"Cooperativa: {codigoEntidad} - Fecha Corte: {fechaCorte}",
                Email = _currentUser.Email,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            var currentFlow = _processFlowStateService.GetLast(agrupador, false);

            if (currentFlow != null && currentFlow.Proceso.Equals(flujoProcesoCarga.Proceso))
                flujoProcesoCarga.Id = currentFlow.Id;

            await _processFlowStateService.AddOrUpdateAsync(flujoProcesoCarga);
        }

        [HttpPost]
        public JsonResult SendEndProcess(string codigoEntidad, string fechaCorte)
        {
            var response = new GeneralResponseVM();
            try
            {
                _processFlowStateService.Delete(codigoEntidad, fechaCorte);
                response = new GeneralResponseVM() { Code = (int)HttpStatusCode.OK, Message = "Se eliminó el proceso." };
                return Json(response);
            }
            catch (Exception e)
            {
                response.Code = 500;
                response.Message = e.Message;
                return Json(response);
            }
        }

        public async Task<JsonResult> GetErrors(string codigoEntidad, string fechaCorte)
        {
            var result = await _formatValidationEventsService.GetErrorsAsync(codigoEntidad, Utilities.DateParse(fechaCorte).Value);
            return Json(result);
        }

        [HttpPost]
        public async Task<FileResult> ExportExcelErrors(string codigoEntidad, string fechaCorte, int downLoadType)
        {
            var listFormatValidation = await _formatValidationEventsService.GetExportDataAsync(codigoEntidad, Utilities.DateParse(fechaCorte).Value, downLoadType);
            return File(_officeService.GenerateSpreadsheetDocument(GetExportList(listFormatValidation.ToList(), downLoadType)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "error_list.xlsx");
        }

        public Guid GenerateGrouperId()
        {
            var newGuid = Guid.NewGuid();
            return newGuid;
        }

        public async Task<string> ValidateCargueExtemporaneo(string fechaCorte, Guid cooperativeTypeId, Guid LoadTypeId)
        {
            try
            {
                return await _maximunDateService.ValidateExtemporaneousLoad(Utilities.DateParse(fechaCorte).Value, cooperativeTypeId, LoadTypeId);
            }
            catch
            {
                return string.Empty;
            }
        }

        private List<object> GetExportList(List<FormatValidationEventVM> listFormatValidation, int downLoadType)
        {
            List<object> dataExportList = new List<object>();

            if (downLoadType == ErrorType.Format.GetHashCode())
            {
                foreach (var item in listFormatValidation)
                {
                    object record = new
                    {
                        Formato = item.FormatName,
                        Campo = item.ColumnName,
                        FilaEnArchivo = item.RowPosition,
                        Descripcion = item.ErrorDescription,
                        FechaValidacion = item.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    dataExportList.Add(record);
                }
            }
            else
            {
                foreach (var item in listFormatValidation)
                {
                    object record = new
                    {
                        Tipo = item.TypeValidation,
                        Formato = item.FormatName,
                        Validacion = item.ErrorDescription,
                        PosibleSolucion = item.PossibleSolution,
                        FechaValidacion = item.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    dataExportList.Add(record);
                }
            }
            return dataExportList;
        }

        private LoadInformationVM GetLoadInformationVM(string entityCode, string cutoffDate, string user, string typeCooperative)
        {
            var obj = new LoadInformationVM
            {
                Id = Guid.Empty,
                EntityCode = entityCode,
                CutoffDate = Utilities.DateParse(cutoffDate).Value,
                CreatedBy = user,
                State = ConstantsLoadInformation.EnProceso,
                CreatedOn = DateTime.Now,
                IsRetransmission = ValidateLaodWithRetrasmission(entityCode, cutoffDate, typeCooperative).Result
            };

            return obj;
        }

        private async Task<bool> ValidateLaodWithRetrasmission(string entityCode, string cutoffDate, string typeCooperative)
        {
            var result = await _retransmissionService.GetRetransmissionAsync(Utilities.DateParse(cutoffDate).Value, int.Parse(entityCode), int.Parse(typeCooperative));

            return result.Count > 0;
        }
    }
}
