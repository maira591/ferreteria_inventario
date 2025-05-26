using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.DataAccess.Model.Formulator;
using Core.Domain.FormatColumnService;
using Core.Domain.FormatService;
using Core.Domain.FormulatorService;
using Core.Domain.HomologationService;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class FormatColumnController : Controller
    {
        #region Private fields
        private readonly IFormatColumnService _columnService;
        private readonly IHomologationService _homologationService;
        private readonly string _apitoken;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IFormulatorService _formulatorService;
        private readonly IFormatService _formatService;
        private const string AceptaNulos = "1";
        private const string NoAceptaNulos = "0";

        #endregion

        public FormatColumnController(IFormatColumnService columnService, IConfigurator configurationService, IMapper mapper, IUserBasicModel userBasicModel,
                                      IFormulatorService formulatorService, IHomologationService homologationService, IFormatService formatService)
        {
            _columnService = columnService;
            _homologationService = homologationService;
            var cfgService = configurationService;
            _apitoken = cfgService.GetKey("ApiToken");
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _formulatorService = formulatorService;
            _formatService = formatService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(Guid formatId)
        {
            return PartialView("_IndexGrid", _mapper.Map<List<FormatColumnModel>>(await _columnService.GetAllByFormatIdAsync(formatId)));
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<Domain.ViewModel.FormatColumnVM>, List<FormatColumnModel>>((await _columnService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create(Guid formatId)
        {
            ViewBag.TitleModal = "Crear columna formato";
            var format = await _formatService.GetByIdAsync(formatId);

            ViewBag.ListFormats = JsonConvert.DeserializeObject<List<SimpleItem>>(await _formulatorService.GetSelectFormats()).ToListItemsGeneric();
            ViewBag.ListHomologations = await HomologationList();
            ViewBag.ListTypes = GetDataTypes().ToListItemsGeneric();
            ViewBag.ListYesNo = GetListYesNo().ToListItemsGeneric();
            FormatColumnModel model = new();
            model.FormatId = format.Id;
            model.FormatName = format.Name;

            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar columna formato";
            var column = _mapper.Map<FormatColumnModel>(await _columnService.GetByIdAsync(new Guid(id)));
            column.FormatName = column.Format.Name;
            column.LongitudDato = column.LongitudDato.Replace(",", ".");
            ViewBag.ListHomologations = await HomologationList();
            ViewBag.ListFormats = JsonConvert.DeserializeObject<List<SimpleItem>>(await _formulatorService.GetSelectFormats()).ToListItemsGeneric();
            ViewBag.ListTypes = GetDataTypes().ToListItemsGeneric();
            ViewBag.ListYesNo = GetListYesNo().ToListItemsGeneric();
            column.SelectAceptaNulos = column.AceptaNulos ? AceptaNulos : NoAceptaNulos;
            return PartialView("_CreateOrEdit", column);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(FormatColumnModel column)
        {
            try
            {
                if (column.Id == Guid.Empty)
                {
                    column.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    column.CreatedOn = DateTime.Now;
                    column.IsNew = true;
                }
                else
                {
                    column.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    column.UpdatedOn = DateTime.Now;
                }
                column.LongitudDato = Common.DecimalValue(column.LongitudDato);
                column.AceptaNulos = column.SelectAceptaNulos == AceptaNulos;
                await _columnService.AddOrUpdateAsync(_mapper.Map<Domain.ViewModel.FormatColumnVM>(column));

                return Json(JsonResponseFactory.SuccessResponse("Columna guardada correctamente."));
            }
            catch (Exception e)
            {
                var err = e.Message;
                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El nombre de columna ingresada ya existe."));
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
                await _columnService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó la columna correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("No fue posible elimiar el registro."));
            }
        }

        private List<SimpleItem> GetDataTypes()
        {
            List<SimpleItem> DataTypeList = new List<SimpleItem>
            {
                new SimpleItem{ Value = "VARCHAR2", Text= "VARCHAR2", Type="VARCHAR2"},
                new SimpleItem{ Value = "DATE", Text= "DATE", Type="DATE"},
                new SimpleItem{ Value = "NUMBER", Text= "NUMBER", Type="NUMBER"}
            };
            return DataTypeList;
        }
        private List<SimpleItem> GetListYesNo()
        {
            List<SimpleItem> DataTypeList = new List<SimpleItem>
            {
                new SimpleItem{ Value = "0", Text= "NO"},
                new SimpleItem{ Value = "1", Text= "SI"}
            };
            return DataTypeList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(FormatColumnModel column)
        {
            string message = string.Empty;

            message = await _columnService.OrderValidation(_mapper.Map<Domain.ViewModel.FormatColumnVM>(column));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }



        private async Task<List<SelectListItem>> HomologationList()
        {
            List<Domain.ViewModel.HomologationVM> homologations = (await _homologationService.GetAllAsync()).ToList();
            var list = Common.LoadList(homologations, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }
    }
}