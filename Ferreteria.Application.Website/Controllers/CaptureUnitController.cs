using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CaptureUnitService;
using Core.Domain.OfficeService;
using Core.Domain.ViewModel;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class CaptureUnitController : Controller
    {
        #region Private fields
        private readonly ICaptureUnitService _captureUnitService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IOfficeService _officeService;
        #endregion

        public CaptureUnitController(ICaptureUnitService captureUnitService, IConfigurator configurationService, IMapper mapper, IUserBasicModel userBasicModel, IOfficeService officeService)
        {
            _captureUnitService = captureUnitService;
            var cfgService = configurationService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _officeService = officeService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<CaptureUnitVM>, List<CaptureUnitModel>>((await _captureUnitService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear unidad de captura";
            return PartialView("_CreateOrEdit", new CaptureUnitModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar unidad de captura";
            var captureUnit = await _captureUnitService.GetByIdAsync(new Guid(id));
            CaptureUnitModel model = _mapper.Map<CaptureUnitModel>(captureUnit);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(CaptureUnitModel captureUnit)
        {
            try
            {
                if (captureUnit.Id == Guid.Empty)
                {
                    captureUnit.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    captureUnit.CreatedOn = DateTime.Now;
                    captureUnit.IsNew = true;
                }
                else
                {
                    captureUnit.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    captureUnit.UpdatedOn = DateTime.Now;
                }
                await _captureUnitService.AddOrUpdateAsync(_mapper.Map<CaptureUnitVM>(captureUnit));

                return Json(JsonResponseFactory.SuccessResponse("Unidad de captura guardada correctamente."));
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
                await _captureUnitService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó la unidad de captura correctamente."));
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
        public async Task<JsonResult> Validations(CaptureUnitModel captureUnit)
        {
            string message = string.Empty;

            message = await _captureUnitService.Validations(_mapper.Map<CaptureUnitVM>(captureUnit));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        public async Task<ActionResult> ExportCaptureUnit(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var captureUnits = (await _captureUnitService.GetAllAsync()).ToList();

            var newCaptureUnits = (from captureUnit in captureUnits select new { Codigo = captureUnit.Code, Nombre = captureUnit.Name, Activo = captureUnit.IsEnabled ? "SI" : "NO" }).OrderBy(t => t.Codigo).ToList<object>();
            sheetsData.Add(newCaptureUnits);
            sheetsName.Add("Unidades de Captura");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}