using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.DataAccess.Model.Formulator;
using Core.DataAccess.Model.Generics;
using Core.Domain.FormulatorService;
using Core.Domain.OfficeService;
using Core.Domain.ViewModel;
using Core.Domain.ViewModel.Formulator;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class ValidationController : Controller
    {
        #region Private fields
        private readonly IFormulatorService _formulatorService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IOfficeService _officeService;

        #endregion

        #region Constructor

        public ValidationController(IFormulatorService formulatorService, IMapper mapper, IUserBasicModel userBasicModel, IOfficeService officeService)
        {
            _formulatorService = formulatorService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _officeService = officeService;
        }

        #endregion

        public async Task<JsonResult> GetOperators()
        {
            var comparisionOperators = await _formulatorService.ComparisonOperators();
            var arithmeticOperators = await _formulatorService.ArithmeticOperators();
            var logicOperators = await _formulatorService.LogicOperators();
            var constants = await _formulatorService.Constants();
            var typeFormats = await _formulatorService.TypesFormats();
            var columns = await _formulatorService.Columns();
            var formats = await _formulatorService.Formats();

            return Json(new
            {
                comparisionOperators,
                arithmeticOperators,
                logicOperators,
                constants,
                columns,
                formats,
                typeFormats
            });
        }

        // GET: Management/Validation
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            return PartialView("_IndexGrid", await _formulatorService.GetAllAsync());
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ValidationModel validation = new ValidationModel();
            ViewBag.ListTypes = JsonConvert.DeserializeObject<List<SimpleItem>>(await _formulatorService.Types()).ToListItemsGeneric();
            ViewBag.TypeFormats = JsonConvert.DeserializeObject<List<DataAccess.Model.Formulator.Type>>(await _formulatorService.TypesFormats()).ToListItemsGenericWithId();
            ViewBag.ListFormats = JsonConvert.DeserializeObject<List<SimpleItem>>(await _formulatorService.Formats()).ToListItemsGeneric();
            ViewBag.TitlePage = "Crear validación";
            return View("CreateOrEdit");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            var validation = await _formulatorService.GetByIdAsync(new Guid(id));
            ViewBag.ListTypes = JsonConvert.DeserializeObject<List<SimpleItem>>(await _formulatorService.Types()).ToListItemsGeneric();
            ViewBag.ListFormats = JsonConvert.DeserializeObject<List<SimpleItem>>(await _formulatorService.FormatsByFotmatTypeId(validation.FormatTypeId)).ToListItemsGeneric();
            ViewBag.TypeFormats = JsonConvert.DeserializeObject<List<DataAccess.Model.Formulator.Type>>(await _formulatorService.TypesFormats()).ToListItemsGenericWithId();
            ViewBag.TitlePage = "Editar validación";
            return PartialView("CreateOrEdit", validation);
        }


        [HttpPost]
        public async Task<JsonResult> SaveInfo(ValidationVM validation)
        {
            GenericResponse<ValidationVM> response;
            var user = _userBasicModel.GetCurrentUser();

            MessageVM message = new MessageVM()
            {
                Id = validation.MessageId,
                Name = validation.Description
            };

            try
            {
                if (validation.Id == Guid.Empty)
                {
                    validation.CreatedBy = user.Id.ToString();
                    message.CreatedBy = user.Id.ToString();
                    validation.CreatedOn = DateTime.Now;
                }
                else
                {
                    validation.UpdatedBy = user.Id.ToString();
                    message.UpdatedBy = user.Id.ToString();
                    validation.UpdatedOn = DateTime.Now;
                }
                response = await _formulatorService.Convertir(validation, _mapper.Map<UserModel>(user), message);
            }
            catch (Exception ex)
            {
                response = GenericResponse<ValidationVM>.GetErrorResponse(500, ex);
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<ActionResult> ExportValidations(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var validations = (await _formulatorService.GetAllAsync()).ToList();

            var newPeriodicities = (from validation in validations select new { Formato = validation.Format, Descripcion = validation.Description, Tipo = validation.Type, Correctiva_Preventiva = validation.Error ? "Correctiva" : "Preventiva", FechaDeExpiracíon = validation.ExpirationDate?.ToString("dd/MM/yyyy"), SoloSQL = validation.IsOnlySQL ? "SI" : "NO", Formula = validation.Formula }).OrderBy(t => t.Formato).ToList<object>();
            sheetsData.Add(newPeriodicities);
            sheetsName.Add("Validaciones");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}