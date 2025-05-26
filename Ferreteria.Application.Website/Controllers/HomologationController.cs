using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.HomologationService;
using Core.Domain.HomologationValueService;
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
    public class HomologationController : Controller
    {
        #region Private fields

        private readonly IHomologationService _homologationService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ILogService _logService;
        private readonly IOfficeService _officeService;
        private readonly IHomologationValueService _homologationValueService;

        #endregion

        #region Constructor
        public HomologationController(IHomologationService homologationService, IMapper mapper, IUserBasicModel userBasicModel, ILogService logService,
            IOfficeService officeService, IHomologationValueService homologationValueService)
        {
            _homologationService = homologationService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _logService = logService;
            _officeService = officeService;
            _homologationValueService = homologationValueService;
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
            var model = _mapper.Map<List<HomologationVM>, List<HomologationModel>>((await _homologationService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear homologación";
            return PartialView("_CreateOrEdit");
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar homologación";
            var homologation = await _homologationService.GetByIdAsync(new Guid(id));

            return PartialView("_CreateOrEdit", _mapper.Map<HomologationModel>(homologation));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(HomologationModel homologation)
        {
            try
            {
                if (homologation.Id == Guid.Empty)
                {
                    homologation.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    homologation.CreatedOn = DateTime.Now;
                    homologation.IsNew = true;
                }
                else
                {
                    homologation.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    homologation.UpdatedOn = DateTime.Now;
                }

                await _homologationService.AddOrUpdateAsync(_mapper.Map<HomologationVM>(homologation));

                return Json(JsonResponseFactory.SuccessResponse("Homologación guardada correctamente."));
            }
            catch (Exception ex)
            {
                _logService.Add(LogKey.Error, $"Trace: {ex.StackTrace}, Error Message: {ex.Message}, Entity: {JsonConvert.SerializeObject(homologation)}");
                _logger.Error(string.Concat("Error en HomologationController - CreateOrUpdate: ", ex.Message));
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
                await _homologationService.DeleteAsync(id);
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
                    _logger.Error(string.Concat("Error en HomologationController - Delete: ", ex.Message));
                }
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(HomologationModel homologation)
        {
            string message = string.Empty;

            message = await _homologationService.Validations(_mapper.Map<HomologationVM>(homologation));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> ExportData(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var homologations = (await _homologationService.GetAllAsync()).ToList();
            var homologationValues = (await _homologationValueService.GetAllAsync()).ToList();

            var newHomologations = homologations.Select(x => new { Nombre = x.Name, Descripcion = x.Description }).ToList<object>();
            var newHomologationValues = homologationValues.Select(x => new
            {
                Homologacion = x.Homologation.Name,
                Nombre = x.Name,
                Codigo = x.Value,
                CodigoHomologado = x.ValueApproved
            }).OrderBy(x => x.Homologacion).ToList<object>();

            sheetsData.Add(newHomologations);
            sheetsName.Add("Homologaciones");
            sheetsData.Add(newHomologationValues);
            sheetsName.Add("Valores Homologaciones");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
        #endregion
    }
}