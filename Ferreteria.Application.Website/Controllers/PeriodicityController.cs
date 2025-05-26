using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.OfficeService;
using Core.Domain.PeriodicityService;
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
    public class PeriodicityController : Controller
    {
        #region Private fields
        private readonly IPeriodicityService _periodicityService;
        private readonly string _apitoken;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IOfficeService _officeService;
        #endregion

        public PeriodicityController(IPeriodicityService periodicityService, IConfigurator configurationService, IMapper mapper, IUserBasicModel userBasicModel, IOfficeService officeService)
        {
            _periodicityService = periodicityService;
            var cfgService = configurationService;
            _apitoken = cfgService.GetKey("ApiToken");
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
            var model = _mapper.Map<List<PeriodicityVM>, List<PeriodicityModel>>((await _periodicityService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear periodicidad";
            return PartialView("_CreateOrEdit", new PeriodicityModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar periodicidad";
            var periodicity = await _periodicityService.GetByIdAsync(new Guid(id));
            PeriodicityModel model = _mapper.Map<PeriodicityModel>(periodicity);
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(PeriodicityModel periodicity)
        {
            try
            {
                periodicity.AppId = new Guid(_apitoken);
                if (periodicity.Id == Guid.Empty)
                {
                    periodicity.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    periodicity.CreatedOn = DateTime.Now;
                    periodicity.IsNew = true;
                }
                else
                {
                    periodicity.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    periodicity.UpdatedOn = DateTime.Now;
                }
                await _periodicityService.AddOrUpdateAsync(_mapper.Map<PeriodicityVM>(periodicity));

                return Json(JsonResponseFactory.SuccessResponse("Periodicidad guardada correctamente."));
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
                await _periodicityService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó la periodicidad correctamente."));
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
        public async Task<JsonResult> Validations(PeriodicityModel periodicity)
        {
            string message = string.Empty;

            message = await _periodicityService.Validations(_mapper.Map<PeriodicityVM>(periodicity));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }


        [HttpPost]
        public async Task<ActionResult> ExportPeriodicity(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var periodicities = (await _periodicityService.GetAllAsync()).ToList();

            var newPeriodicities = (from periocidity in periodicities select new { Nombre = periocidity.Name, NumeroDeDias = periocidity.Days, Descripcion = periocidity.Description, Activo = periocidity.IsEnabled ? "SI" : "NO" }).OrderBy(t => t.Nombre).ToList<object>();
            sheetsData.Add(newPeriodicities);
            sheetsName.Add("Periodicidades");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}